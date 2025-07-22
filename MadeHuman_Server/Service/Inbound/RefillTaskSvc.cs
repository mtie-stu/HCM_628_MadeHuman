using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.WareHouse;
using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.Inbound;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Inbound
{
    public interface IRefillTaskService
    {
        Task<RefillTaskFullViewModel> CreateAsync(RefillTaskFullViewModel model, string UserId  );
        Task<RefillTaskFullViewModel> UpdateAsync(Guid id, RefillTaskFullViewModel model);
        Task<List<RefillTaskFullViewModel>> GetAllAsync();
        Task<List<RefillTaskDetailWithHeaderViewModel>> GetAllDetailsAsync();
        Task<RefillTaskFullViewModel?> GetByIdAsync(Guid id);
        Task<RefillTaskFullViewModel?> AssignRefillTaskToCurrentUserAsync();
        Task<List<string>> ValidateRefillTaskScanAsync(ScanRefillTaskValidationRequest request);
        Task StoreRefillTaskDetailAsync(Guid refillTaskId, Guid detailId, Guid userTaskId);
    }
    public class RefillTaskService : IRefillTaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserTaskSvc _usertaskservice;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public RefillTaskService(ApplicationDbContext context,IUserTaskSvc userTaskSvc, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _usertaskservice = userTaskSvc;
            _httpContextAccessor = httpContextAccessor;
        }

            public async Task<RefillTaskFullViewModel> CreateAsync(RefillTaskFullViewModel vm, string UserId)
            {
                var errors = new List<string>();

                var skuMap = new Dictionary<string, Guid>(); // Cache SKU → Id để tránh query nhiều lần

                foreach (var d in vm.Details)
                {
                    Guid productSkuId;

                    // ✅ 1. Nếu có ProductSKUId thì dùng, nếu không thì tra SKU
                    if (d.ProductSKUId.HasValue)
                    {
                        productSkuId = d.ProductSKUId.Value;
                    }
                    else if (!string.IsNullOrWhiteSpace(d.SKU))
                    {
                        if (skuMap.TryGetValue(d.SKU, out var cachedId))
                        {
                            productSkuId = cachedId;
                        }
                        else
                        {
                            var skuEntity = await _context.ProductSKUs
                                .FirstOrDefaultAsync(p => p.SKU == d.SKU);

                            if (skuEntity == null)
                            {
                                errors.Add($"❌ Không tìm thấy SKU: {d.SKU}");
                                continue;
                            }

                            productSkuId = skuEntity.Id;
                            skuMap[d.SKU] = productSkuId; // cache lại
                        }

                        d.ProductSKUId = productSkuId; // gán lại để tạo nhiệm vụ
                    }
                    else
                    {
                        errors.Add("❌ Vui lòng nhập ProductSKUId hoặc SKU.");
                        continue;
                    }

                    // ✅ 2. Kiểm tra FromLocation có đúng SKU
                    var fromInventory = await _context.Inventory.FirstOrDefaultAsync(i =>
                        i.WarehouseLocationId == d.FromLocation && i.ProductSKUId == productSkuId);

                    if (fromInventory == null)
                    {
                        errors.Add($"❌ FromLocation không có SKU {productSkuId}.");
                        continue;
                    }

                    if (fromInventory.StockQuantity < d.Quantity)
                    {
                        errors.Add($"❌ FromLocation không đủ hàng SKU {productSkuId}. Yêu cầu: {d.Quantity}, hiện có: {fromInventory.StockQuantity}");
                    }

                    // ✅ 3. Kiểm tra ToLocation có đúng SKU
                    var toInventory = await _context.Inventory.FirstOrDefaultAsync(i =>
                        i.WarehouseLocationId == d.ToLocation/* && i.ProductSKUId == productSkuId*/);

                    if (toInventory == null)
                    {
                        errors.Add($"❌ ToLocation không có SKU {productSkuId}. Vui lòng tạo tồn kho đích.");
                    }
                }

                if (errors.Any())
                    throw new InvalidOperationException(string.Join("\n", errors));
//                var userTaskExists = await _context.UsersTasks
//    .AnyAsync(x => x.Id == vm.UserTaskId);

//if (!userTaskExists)
//    throw new InvalidOperationException("❌ UserTaskId không tồn tại.");

                // ✅ Hợp lệ → tạo task
                var task = new RefillTasks
                {
                    Id = Guid.NewGuid(),
                    //UserTaskId = vm.UserTaskId!.Value, // ✅ BỔ SUNG DÒNG NÀY
                    StatusRefillTasks = StatusRefillTasks.Incomplete,
                    CreateAt = DateTime.UtcNow,
                    CreateBy = UserId,
                    RefillTaskDetails = vm.Details.Select(d => new RefillTaskDetails
                    {
                        Id = Guid.NewGuid(),
                        ProductSKUId = d.ProductSKUId!.Value,
                        FromLocation = d.FromLocation,
                        ToLocation = d.ToLocation,
                        Quantity = d.Quantity
                    }).ToList()
                };

                _context.RefillTasks.Add(task);
                await _context.SaveChangesAsync();

                vm.Id = task.Id;
                return vm;
            }

        public async Task<RefillTaskFullViewModel> UpdateAsync(Guid id, RefillTaskFullViewModel vm)
        {
            var task = await _context.RefillTasks
                .Include(x => x.RefillTaskDetails)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
                throw new Exception("Không tìm thấy RefillTask");

            //task.LowStockId = vm.LowStockId;
            //task.UserTaskId = vm.UserTaskId;

            // Xoá chi tiết cũ
            _context.RefillTaskDetails.RemoveRange(task.RefillTaskDetails);

            // Thêm chi tiết mới
            task.RefillTaskDetails = vm.Details.Select(d => new RefillTaskDetails
            {
                Id = Guid.NewGuid(),
                FromLocation = d.FromLocation,
                ToLocation = d.ToLocation,
                Quantity = d.Quantity
            }).ToList();

            await _context.SaveChangesAsync();
            return vm;
        }

        public async Task<List<RefillTaskFullViewModel>> GetAllAsync()
        {
            return await _context.RefillTasks
                .Include(x => x.RefillTaskDetails)
                .Select(x => new RefillTaskFullViewModel
                {
                    Id = x.Id,
                    LowStockId = x.LowStockId,
                    UserTaskId = x.UserTaskId,
                    CreateAt = x.CreateAt,
                    CreateBy = x.CreateBy,
                    Details = x.RefillTaskDetails.Select(d => new RefillTaskFullViewModel.RefillTaskDetailItem
                    {
                        Id = d.Id,
                        FromLocation = d.FromLocation,
                        ToLocation = d.ToLocation,
                        Quantity = d.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<RefillTaskFullViewModel?> GetByIdAsync(Guid id)
        {
            return await _context.RefillTasks
                .Include(x => x.RefillTaskDetails)
                .Where(x => x.Id == id)
                .Select(x => new RefillTaskFullViewModel
                {
                    Id = x.Id,
                    LowStockId = x.LowStockId,
                    UserTaskId = x.UserTaskId,
                    CreateAt = x.CreateAt,
                    CreateBy = x.CreateBy,
                    Details = x.RefillTaskDetails.Select(d => new RefillTaskFullViewModel.RefillTaskDetailItem
                    {
                        Id = d.Id,
                        FromLocation = d.FromLocation,
                        ToLocation = d.ToLocation,
                        Quantity = d.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<List<RefillTaskDetailWithHeaderViewModel>> GetAllDetailsAsync()
        {
            return await _context.RefillTaskDetails
                .Include(d => d.RefillTasks)
                .Select(d => new RefillTaskDetailWithHeaderViewModel
                {
                    RefillTaskId = d.RefillTaskId,
                    CreateBy = d.RefillTasks.CreateBy,
                    CreateAt = d.RefillTasks.CreateAt,
                    LowStockId = d.RefillTasks.LowStockId,
                    UserTaskId = d.RefillTasks.UserTaskId,

                    DetailId = d.Id,
                    FromLocation = d.FromLocation,
                    ToLocation = d.ToLocation,
                    Quantity = d.Quantity
                })
                .ToListAsync();
        }
        public async Task<RefillTaskFullViewModel?> AssignRefillTaskToCurrentUserAsync()
        {
            // 1. Lấy userId hiện tại
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("❌ Không xác định được người dùng hiện tại.");

            // 2. Lấy UserTaskId theo userId và ngày hiện tại
            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
                throw new InvalidOperationException("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");

            // 3. Tìm RefillTask đầu tiên chưa được nhận (UserTaskId null hoặc Guid.Empty)
            var task = await _context.RefillTasks
                .Include(x => x.RefillTaskDetails)
                .Where(x => x.UserTaskId == null || x.UserTaskId == Guid.Empty)
                .OrderBy(x => x.CreateAt) // Ưu tiên task tạo sớm hơn
                .FirstOrDefaultAsync();

            if (task == null)
                return null; // 🎉 Không còn nhiệm vụ nào chưa gán

            // 4. Gán UserTaskId cho RefillTask
            task.UserTaskId = userTaskId;

            await _context.SaveChangesAsync();

            // 5. Trả về ViewModel
            return new RefillTaskFullViewModel
            {
                Id = task.Id,
                LowStockId = task.LowStockId,
                UserTaskId = task.UserTaskId,
                CreateAt = task.CreateAt,
                CreateBy = task.CreateBy,
                Details = task.RefillTaskDetails.Select(d => new RefillTaskFullViewModel.RefillTaskDetailItem
                {
                    Id = d.Id,
                    FromLocation = d.FromLocation,
                    ToLocation = d.ToLocation,
                    Quantity = d.Quantity
                }).ToList()
            };
        }
        public async Task<List<string>> ValidateRefillTaskScanAsync(ScanRefillTaskValidationRequest request)
        {
            var errors = new List<string>();

            var task = await _context.RefillTasks
                .Include(t => t.RefillTaskDetails)
                .FirstOrDefaultAsync(t => t.Id == request.RefillTaskId);

            if (task == null)
            {
                errors.Add("❌ Không tìm thấy RefillTask.");
                return errors;
            }

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                errors.Add("❌ Không xác định được người dùng hiện tại.");
                return errors;
            }

            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
            {
                errors.Add("❌ Không tìm thấy phân công công việc hôm nay.");
                return errors;
            }

            if (task.UserTaskId != null && task.UserTaskId != Guid.Empty)
            {
                if (task.UserTaskId != userTaskId)
                {
                    errors.Add("❌ Nhiệm vụ đã được gán cho người khác.");
                    return errors;
                }
            }
            else
            {
                task.UserTaskId = userTaskId.Value;
                await _context.SaveChangesAsync();
            }

            var detail = task.RefillTaskDetails.FirstOrDefault(d => d.Id == request.RefillTaskDetailId);
            if (detail == null)
            {
                errors.Add("❌ Không tìm thấy chi tiết nhiệm vụ.");
                return errors;
            }

            // Kiểm tra vị trí
            if (!string.IsNullOrWhiteSpace(request.FromLocationName))
            {
                var actualFrom = await _context.WarehouseLocations
                    .Where(w => w.Id == detail.FromLocation)
                    .Select(w => w.NameLocation)
                    .FirstOrDefaultAsync() ?? "(null)";

                if (!actualFrom.Equals(request.FromLocationName, StringComparison.OrdinalIgnoreCase))
                    errors.Add($"❌ FromLocation không khớp. Hệ thống: {actualFrom}, bạn nhập: {request.FromLocationName}");
            }

            if (!string.IsNullOrWhiteSpace(request.ToLocationName))
            {
                var actualTo = await _context.WarehouseLocations
                    .Where(w => w.Id == detail.ToLocation)
                    .Select(w => w.NameLocation)
                    .FirstOrDefaultAsync() ?? "(null)";

                if (!actualTo.Equals(request.ToLocationName, StringComparison.OrdinalIgnoreCase))
                    errors.Add($"❌ ToLocation không khớp. Hệ thống: {actualTo}, bạn nhập: {request.ToLocationName}");
            }

            // SKU
            if (!string.IsNullOrWhiteSpace(request.SKU))
            {
                var sku = await _context.ProductSKUs
                    .Where(p => p.Id == detail.ProductSKUId)
                    .Select(p => p.SKU)
                    .FirstOrDefaultAsync() ?? "(null)";

                if (!sku.Equals(request.SKU, StringComparison.OrdinalIgnoreCase))
                    errors.Add($"❌ SKU không khớp. Hệ thống: {sku}, bạn nhập: {request.SKU}");
            }

            // Quantity
            if (request.Quantity.HasValue && request.Quantity != detail.Quantity)
            {
                errors.Add($"❌ Số lượng không khớp. Hệ thống: {detail.Quantity}, bạn nhập: {request.Quantity}");
            }

            var enoughInfo = !string.IsNullOrWhiteSpace(request.SKU)
                          && !string.IsNullOrWhiteSpace(request.FromLocationName)
                          && !string.IsNullOrWhiteSpace(request.ToLocationName)
                          && request.Quantity.HasValue;

            if (!errors.Any() && enoughInfo)
            {
                await StoreRefillTaskDetailAsync(task.Id, detail.Id, userTaskId.Value);
                errors.Add("✅ Quét thành công và xử lý luân chuyển.");
            }
            else if (!errors.Any())
            {
                errors.Add("✅ Dữ liệu hợp lệ nhưng chưa đủ để xử lý.");
            }

            return errors;
        }
        public async Task StoreRefillTaskDetailAsync(Guid refillTaskId, Guid detailId, Guid userTaskId)
        {
            var task = await _context.RefillTasks
                .Include(t => t.RefillTaskDetails)
                .FirstOrDefaultAsync(t => t.Id == refillTaskId)
                ?? throw new Exception("Không tìm thấy RefillTask.");

            var detail = task.RefillTaskDetails.FirstOrDefault(d => d.Id == detailId)
                ?? throw new Exception("Không tìm thấy chi tiết.");

            var skuId = detail.ProductSKUId;

            // Trừ hàng từ FromLocation
            var fromInventory = await _context.Inventory.FirstOrDefaultAsync(i =>
                i.WarehouseLocationId == detail.FromLocation && i.ProductSKUId == skuId);

            if (fromInventory == null || fromInventory.StockQuantity < detail.Quantity)
                throw new Exception("Tồn kho nguồn không đủ.");

            fromInventory.StockQuantity -= detail.Quantity;
            fromInventory.LastUpdated = DateTime.UtcNow;

            // Cộng hàng vào ToLocation
            var toInventory = await _context.Inventory.FirstOrDefaultAsync(i =>
                i.WarehouseLocationId == detail.ToLocation && i.ProductSKUId == skuId);

            if (toInventory == null)
            {
                toInventory = new Inventory
                {
                    Id = Guid.NewGuid(),
                    ProductSKUId = skuId,
                    WarehouseLocationId = detail.ToLocation,
                    StockQuantity = detail.Quantity,
                    LastUpdated = DateTime.UtcNow
                };
                _context.Inventory.Add(toInventory);
            }
            else
            {
                toInventory.StockQuantity += detail.Quantity;
                toInventory.LastUpdated = DateTime.UtcNow;
            }

            // Ghi log
            _context.InventoryLogs.Add(new InventoryLogs
            {
                Id = Guid.NewGuid(),
                InventoryId = toInventory.Id,
                StockQuantity = fromInventory.StockQuantity,
                ChangeQuantity = detail.Quantity,
                RemainingQuantity = toInventory.StockQuantity,
                ActionInventoryLogs = ActionInventoryLogs.Refill,
                ChangeBy = userTaskId.ToString(),
                Time = DateTime.UtcNow
            });

            // Cập nhật LowStockAlert ở ToLocation nếu có
            var alert = await _context.LowStockAlerts
                .FirstOrDefaultAsync(a => a.WarehouseLocationId == detail.ToLocation);

            if (alert != null)
            {
                alert.CurrentQuantity += detail.Quantity;
                alert.StatusLowStockAlerts = alert.CurrentQuantity switch
                {
                    <= 0 => StatusLowStockAlerts.Empty,
                    < 10 => StatusLowStockAlerts.Warning,
                    _ => StatusLowStockAlerts.Normal
                };
            }

            // Nếu tất cả đã thực hiện → task hoàn thành
            var isDone = task.RefillTaskDetails.All(d =>
                _context.Inventory.Any(i => i.WarehouseLocationId == d.ToLocation && i.ProductSKUId == d.ProductSKUId && i.StockQuantity >= d.Quantity));

            if (isDone)
            {
                task.StatusRefillTasks = StatusRefillTasks.Completed;

                var totalQty = task.RefillTaskDetails.Sum(d => d.Quantity);
                var userTask = await _context.UsersTasks.FirstOrDefaultAsync(u => u.Id == userTaskId);
                if (userTask != null)
                {
                    userTask.TotalKPI += totalQty;
                    userTask.HourlyKPIs += totalQty;
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
