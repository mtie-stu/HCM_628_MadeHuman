//using MadeHuman_Server.Data;
//using MadeHuman_Server.Model.Inbound;
//using MadeHuman_Server.Model.WareHouse;
//using Madehuman_Share.ViewModel.Inbound;
//using Microsoft.EntityFrameworkCore;

//namespace MadeHuman_Server.Service.Inbound
//{
//    public interface IInboundTaskSvc
//    {
//        Task<InboundTasks> CreateInboundTaskAsync(CreateInboundTaskViewModel vm, string userId);
//    }
//    public class InboundTaskSvc : IInboundTaskSvc
//    {
//        private readonly ApplicationDbContext _context;

//        public InboundTaskSvc(ApplicationDbContext context)
//        {
//            _context = context;
//        }


//        public async Task<InboundTasks> CreateInboundTaskAsync(CreateInboundTaskViewModel vm, string userId)
//        {
//            // 1. Tìm phiếu nhập
//            var receipt = await _context.InboundReceipt
//                .Include(r => r.InboundReceiptItems)
//                    .ThenInclude(i => i.ProductSKUs)
//                .FirstOrDefaultAsync(r => r.Id == vm.InboundReceiptId);

//            if (receipt == null)
//                throw new ArgumentException("InboundReceiptId không tồn tại.");

//            if (receipt.Status != StatusReceipt.Confirmed)
//                throw new InvalidOperationException("InboundReceipt phải ở trạng thái đã xác nhận.");

//            if (receipt.InboundReceiptItems == null || !receipt.InboundReceiptItems.Any())
//                throw new InvalidOperationException("Phiếu nhập không có item nào.");

//            Console.WriteLine($"Tổng số InboundReceiptItems: {receipt.InboundReceiptItems.Count}");

//            // 2. Tạo InboundTask
//            var taskId = Guid.NewGuid();
//            var task = new InboundTasks
//            {
//                Id = taskId,
//                CreateBy = userId,
//                CreateAt = DateTime.UtcNow,
//                Status = Status.Created,
//                InboundReceiptId = vm.InboundReceiptId,
//            };

//            _context.InboundTasks.Add(task);

//            // 3. Tìm danh sách kho trống
//            var availableWarehouses = await _context.WarehouseLocations
//                .Where(w => w.StatusWareHouse == StatusWareHouse.Empty)
//                .Take(receipt.InboundReceiptItems.Count)
//                .ToListAsync();

//            if (availableWarehouses.Count < receipt.InboundReceiptItems.Count)
//                throw new InvalidOperationException("Không đủ vị trí kho trống để chứa các lô hàng.");

//            // 4. Tạo ProductBatches và gán kho
//            int index = 0;
//            var productBatches = new List<ProductBatches>();

//            foreach (var item in receipt.InboundReceiptItems)
//            {
//                var warehouse = availableWarehouses[index++];

//                productBatches.Add(new ProductBatches
//                {
//                    Id = Guid.NewGuid(),
//                    Quantity = item.Quantity,
//                    ProductSKUId = item.ProductSKUId,
//                    StatusProductBatches = StatusProductBatches.UnStored,
//                    InboundTaskId = taskId,
//                    WarehouseLocationId = warehouse.Id
//                });

//                warehouse.StatusWareHouse = StatusWareHouse.Booked;
//            }

//            // Add 1 lần
//            _context.ProductBatches.AddRange(productBatches);





//            await _context.SaveChangesAsync();

//            // Debug: kiểm tra lại số lượng batch đã tạo
//            var batchCount = await _context.ProductBatches.CountAsync(b => b.InboundTaskId == taskId);
//            Console.WriteLine($"Đã tạo {batchCount} product batches cho task {taskId}");

//            return task;
//        }



//    }
//}


using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.WareHouse;
using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.Inbound;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Inbound
{
    public interface IInboundTaskSvc
    {
        Task<InboundTasks> CreateInboundTaskAsync(CreateInboundTaskViewModel vm, string userId);
        Task<GetInboundTaskById_Viewmodel> GetInboundTaskByIdAsync(Guid inboundTaskId);
        Task<List<string>> ValidateInboundProductScanAsync(ScanInboundTaskValidationRequest request);
        Task StoreProductBatchAsync(Guid inboundTaskId, Guid productBatchId, Guid userTaskId);
    }

    public class InboundTaskSvc : IInboundTaskSvc
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserTaskSvc _usertaskservice;
        public InboundTaskSvc(ApplicationDbContext context,IUserTaskSvc userTaskSvc, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _usertaskservice = userTaskSvc;
               _httpContextAccessor = httpContextAccessor;  
        }

        public async Task<InboundTasks> CreateInboundTaskAsync(CreateInboundTaskViewModel vm, string userId)
        {
            // 1. Xác minh phiếu nhập
            var receipt = await _context.InboundReceipt
                .Include(r => r.InboundReceiptItems)
                .FirstOrDefaultAsync(r => r.Id == vm.InboundReceiptId);

            if (receipt == null)
                throw new ArgumentException("InboundReceiptId không tồn tại.");

            if (receipt.Status != StatusReceipt.Confirmed)
                throw new InvalidOperationException("Phiếu nhập chưa được xác nhận.");

            var items = receipt.InboundReceiptItems.ToList(); // ✅ Chuyển sang List để dùng [i]
            if (!items.Any())
                throw new InvalidOperationException("Phiếu nhập không có item.");
            // ✅ Gán thời gian nhận hàng
            receipt.ReceivedAt = DateTime.UtcNow;
            // 2. Kiểm tra kho
            // 2. Lấy Zone "Inbound"
            var inboundZone = await _context.WarehouseZones
                .FirstOrDefaultAsync(z => z.Name == "Inbound");

            if (inboundZone == null)
                throw new InvalidOperationException("Không tìm thấy zone 'Inbound'.");

            // 3. Lọc danh sách kho trống trong zone 'Inbound'
            var warehouseSlots = await _context.WarehouseLocations
                .Where(x => x.StatusWareHouse == StatusWareHouse.Empty && x.ZoneId == inboundZone.Id)
                .Take(items.Count)
                .ToListAsync();

            if (warehouseSlots.Count < items.Count)
                throw new InvalidOperationException("Không đủ kho trống trong khu vực 'Inbound'.");


            if (warehouseSlots.Count < items.Count)
                throw new InvalidOperationException("Không đủ kho trống để chứa sản phẩm.");

            // 3. Tạo Task
            var taskId = Guid.NewGuid();
            var task = new InboundTasks
            {
                Id = taskId,
                CreateAt = DateTime.UtcNow,
                CreateBy = userId,
                Status = Status.Created,
                InboundReceiptId = vm.InboundReceiptId
            };
            await _context.InboundTasks.AddAsync(task);
            await _context.SaveChangesAsync(); // Lưu để có FK hợp lệ

            // 4. Tạo batches + cập nhật kho
            var batches = new List<ProductBatches>();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var slot = warehouseSlots[i];

                batches.Add(new ProductBatches
                {
                    Id = Guid.NewGuid(),
                    ProductSKUId = item.ProductSKUId,
                    Quantity = item.Quantity,
                    StatusProductBatches = StatusProductBatches.UnStored,
                    InboundTaskId = taskId,
                    WarehouseLocationId = slot.Id
                });

                slot.StatusWareHouse = StatusWareHouse.Booked;
            }

            await _context.ProductBatches.AddRangeAsync(batches);
            await _context.SaveChangesAsync(); // Lưu cả batch + cập nhật kho

            return task;
        }

        public async Task<GetInboundTaskById_Viewmodel> GetInboundTaskByIdAsync(Guid inboundTaskId)
        {
            var task = await _context.InboundTasks
                .Include(t => t.ProductBatches)
                    .ThenInclude(pb => pb.ProductSKUs)
                .Include(t => t.ProductBatches)
                    .ThenInclude(pb => pb.WarehouseLocation)
                .FirstOrDefaultAsync(t => t.Id == inboundTaskId);

            if (task == null) return null;

            return new GetInboundTaskById_Viewmodel
            {
                InboundTaskId = task.Id,
                CreateBy = task.CreateBy,
                CreateAt = task.CreateAt,
                Status = task.Status.ToString(),
                ProductBatches = task.ProductBatches.Select(batch => new BatchInfoViewModel
                {
                    ProductBatchId = batch.Id,
                    Quantity = batch.Quantity,
                    StatusProductBatch = batch.StatusProductBatches.ToString(),
                    SKU = batch.ProductSKUs?.SKU,
                    NameLocation = batch.WarehouseLocation?.NameLocation
                }).ToList()
            };
        }
        public async Task<List<string>> ValidateInboundProductScanAsync(ScanInboundTaskValidationRequest request)
        {
            var errors = new List<string>();

            // 1. Kiểm tra InboundTask
            var task = await _context.InboundTasks
                .Include(t => t.ProductBatches)
                    .ThenInclude(pb => pb.ProductSKUs)
                .Include(t => t.ProductBatches)
                    .ThenInclude(pb => pb.WarehouseLocation)
                .FirstOrDefaultAsync(t => t.Id == request.InboundTaskId);

            if (task == null)
            {
                errors.Add("❌ Không tìm thấy InboundTask.");
                return errors;
            }

            // 2. Lấy userId hiện tại
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                errors.Add("❌ Không xác định được người dùng hiện tại.");
                return errors;
            }

            // 3. Lấy UserTaskId hiện tại
            var currentUserTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (currentUserTaskId == null)
            {
                errors.Add("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");
                return errors;
            }

            // 4. So sánh hoặc gán UserTaskId vào InboundTask
            if (task.UserTaskId != null && task.UserTaskId != Guid.Empty)
            {
                if (task.UserTaskId != currentUserTaskId)
                {
                    errors.Add("❌ Nhiệm vụ này đã được xác nhận cho người dùng khác.");
                    return errors;
                }
            }
            else
            {
                task.UserTaskId = currentUserTaskId.Value;
                await _context.SaveChangesAsync();
            }


            // 5. Tìm đúng ProductBatch
            var batch = task.ProductBatches.FirstOrDefault(pb => pb.Id == request.ProductBatchId);
            if (batch == null)
            {
                errors.Add($"❌ Không tìm thấy ProductBatchId: {request.ProductBatchId} trong InboundTask.");
                return errors;
            }

            // 6. Kiểm tra NameLocation nếu có
            if (!string.IsNullOrWhiteSpace(request.NameLocation))
            {
                var actual = batch.WarehouseLocation?.NameLocation ?? "(null)";
                if (!actual.Equals(request.NameLocation, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"❌ Vị trí kho không khớp. Hệ thống: {actual}, bạn nhập: {request.NameLocation}.");
                }
            }

            // 7. Kiểm tra SKU nếu có
            if (!string.IsNullOrWhiteSpace(request.SKU))
            {
                var actual = batch.ProductSKUs?.SKU ?? "(null)";
                if (!actual.Equals(request.SKU, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"❌ SKU không khớp. Hệ thống: {actual}, bạn nhập: {request.SKU}.");
                }
            }

            // 8. Kiểm tra Quantity nếu có
            if (request.Quantity.HasValue)
            {
                if (request.Quantity.Value != batch.Quantity)
                {
                    errors.Add($"❌ Số lượng không khớp. Hệ thống: {batch.Quantity}, bạn nhập: {request.Quantity.Value}.");
                }
            }

            // ✅ Nếu không có lỗi nào và không trống 
            var allFieldsProvided =
        !string.IsNullOrWhiteSpace(request.NameLocation) &&
        !string.IsNullOrWhiteSpace(request.SKU) &&
        request.Quantity.HasValue;

            if (!errors.Any() && allFieldsProvided)
            {
                await StoreProductBatchAsync(task.Id, batch.Id, currentUserTaskId.Value);
                errors.Add("✅ Thông tin hợp lệ, lưu kho thành công.");
            }
            else if (!errors.Any())
            {
                errors.Add("✅ Thông tin hợp lệ, chưa tiến hành lưu kho vì thiếu thông tin.");
            }


            return errors;
        }

        public async Task StoreProductBatchAsync(Guid inboundTaskId, Guid productBatchId, Guid userTaskId)
        {
            var task = await _context.InboundTasks
                .Include(t => t.ProductBatches)
                .FirstOrDefaultAsync(t => t.Id == inboundTaskId);

            if (task == null)
                throw new ArgumentException("❌ Không tìm thấy InboundTask.");

            var batch = task.ProductBatches.FirstOrDefault(b => b.Id == productBatchId);
            if (batch == null)
                throw new ArgumentException("❌ Không tìm thấy ProductBatch trong InboundTask.");

            // 1. Đánh dấu batch đã lưu kho
            batch.StatusProductBatches = StatusProductBatches.Stored;

            // 2. Tìm Inventory theo WarehouseLocationId
            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(i => i.WarehouseLocationId == batch.WarehouseLocationId);

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    Id = Guid.NewGuid(),
                    ProductSKUId = batch.ProductSKUId,
                    WarehouseLocationId = batch.WarehouseLocationId,
                    StockQuantity = batch.Quantity,
                    LastUpdated = DateTime.UtcNow
                };
                _context.Inventory.Add(inventory);
            }
            else
            {
                inventory.StockQuantity += batch.Quantity;
                inventory.LastUpdated = DateTime.UtcNow;
            }

            // 3. Lấy log gần nhất để tính tồn kho trước
            var latestLog = await _context.InventoryLogs
                .Where(log => log.InventoryId == inventory.Id)
                .OrderByDescending(log => log.Time)
                .FirstOrDefaultAsync();

            var previousQuantity = latestLog?.RemainingQuantity ?? 0;

            // 4. Tạo log mới
            var newLog = new InventoryLogs
            {
                Id = Guid.NewGuid(),
                InventoryId = inventory.Id,
                StockQuantity = previousQuantity,
                ChangeQuantity = batch.Quantity,
                RemainingQuantity = previousQuantity + batch.Quantity,
                ActionInventoryLogs = ActionInventoryLogs.Refill,
                ChangeBy = userTaskId.ToString(),
                Time = DateTime.UtcNow
            };

            _context.InventoryLogs.Add(newLog);

            // 5. Nếu tất cả các batch đã Stored → hoàn thành task + cộng KPI
            var allStored = task.ProductBatches.All(b => b.StatusProductBatches == StatusProductBatches.Stored);
            if (allStored)
            {
                task.Status = Status.Completed;

                // 🔥 Tổng quantity của tất cả các batch thuộc task
                var totalQuantity = task.ProductBatches.Sum(b => b.Quantity);

                var userTask = await _context.UsersTasks.FirstOrDefaultAsync(ut => ut.Id == userTaskId);
                if (userTask != null)
                {
                    // Nếu chưa có KPI thì khởi tạo = 0
                    userTask.TotalKPI += totalQuantity;
                    userTask.HourlyKPIs += totalQuantity;
                }
            }

            await _context.SaveChangesAsync();
        }


    }
}
