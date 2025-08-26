// CheckTaskServices.cs
using Google.Apis.Drive.v3.Data;
using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Service.Shop;
using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Outbound
{
    public interface ICheckTaskServices
    {
        Task<PreviewSingleSKUResponse?> PreviewSingleSKUAsync(Guid basketId, string sku);
        Task<(int Code, List<string> Logs)> ValidateMixCheckTaskScanAsync(ValidateMixCheckTaskRequest request);
        Task<CheckTasks> CreateCheckTaskAsync(Guid outboundTaskId);
        Task<CheckTaskFullViewModel> AssignUserTaskToCheckTaskByBasketAsync(Guid basketId);
        Task<List<string>> ValidateCheckTaskScanAsync(ScanCheckTaskRequest request);
        Task<CheckTaskResultViewModel> AssignSlotAsync(AssignSlotRequest request);
        Task<List<string>> ValidateSingleSKUCheckTaskAsync(SingleSKUCheckTaskRequest request);
        Task<IEnumerable<CheckTaskLogViewModel>> GetLogsByCheckTaskIdAsync(Guid checkTaskId);
    }

    public class CheckTaskServices : ICheckTaskServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserTaskSvc _usertaskservice;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPackTaskService _packTaskService;
        private readonly IProductImageService _productImageService;
        private readonly IProductLookupService _productLookup;


        public CheckTaskServices(ApplicationDbContext context, IUserTaskSvc userTaskSvc, IHttpContextAccessor httpContextAccessor, IPackTaskService packTaskService, IProductImageService productImageService, IProductLookupService productLookupService)
        {
            _context = context;
            _usertaskservice = userTaskSvc;
            _httpContextAccessor = httpContextAccessor;
            _packTaskService = packTaskService; 
            _productImageService = productImageService;
            _productLookup = productLookupService;
        }

        public async Task<PreviewSingleSKUResponse?> PreviewSingleSKUAsync(Guid basketId, string sku)
        {
            // Bước 1: Lấy OutboundTaskId từ Basket
            var basket = await _context.Baskets
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == basketId);

            if (basket == null)
                return null;

            // Bước 2: Tìm CheckTask theo OutboundTaskId
            var checkTask = await _context.CheckTasks
                .Include(ct => ct.CheckTaskDetails)
                    .ThenInclude(d => d.OutboundTaskItems)
                        .ThenInclude(oti => oti.OutboundTaskItemDetails)
                .FirstOrDefaultAsync(ct => ct.OutboundTaskId == basket.OutBoundTaskId);

            if (checkTask == null)
                return null;

            // Bước 3: Lấy danh sách SKUId cần check
            var skuIds = checkTask.CheckTaskDetails
                .SelectMany(d => d.OutboundTaskItems.OutboundTaskItemDetails)
                .Select(i => i.ProductSKUId)
                .Distinct()
                .ToList();

            // Bước 4: Tìm SKU khớp
            var matchedSku = await _context.ProductSKUs
                .Include(p => p.Product)
                .Include(p => p.Combo)
                .Where(p => skuIds.Contains(p.Id))
                .ToListAsync();

            var matched = matchedSku
                .FirstOrDefault(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));

            if (matched == null)
                return null;

            // Bước 5: Tính tổng số lượng yêu cầu
            var requiredQty = checkTask.CheckTaskDetails
                .SelectMany(d => d.OutboundTaskItems.OutboundTaskItemDetails)
                .Where(i => i.ProductSKUId == matched.Id)
                .Sum(i => i.Quantity);

            // Bước 6: Lấy ảnh
            var imageDict = await _productImageService.GetImageUrlsByProductSKUIdsAsync(new List<Guid> { matched.Id });
            var imageUrls = imageDict.ContainsKey(matched.Id) ? imageDict[matched.Id] : new List<string>();

            return new PreviewSingleSKUResponse
            {
                SKU = matched.SKU,
                ProductName = matched.Product?.Name ?? matched.Combo?.Name ?? "Unknown",
                RequiredQuantity = requiredQty,
                ImageUrls = imageUrls
            };
        }



        public async Task<(int Code, List<string> Logs)> ValidateMixCheckTaskScanAsync(ValidateMixCheckTaskRequest request)
        {
            var logs = new List<string>();

            var detail = await _context.CheckTaskDetails
                .Include(d => d.OutboundTaskItems)
                    .ThenInclude(oti => oti.OutboundTaskItemDetails)
                .FirstOrDefaultAsync(d => d.Id == request.CheckTaskDetailId);

            if (detail == null)
                return (0, new List<string> { "❌ Không tìm thấy chi tiết kiểm hàng." });

            // Map SKU -> ProductSKUId
            var productSkuId = await _context.ProductSKUs
                .Where(p => p.SKU == request.SKU)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            if (productSkuId == Guid.Empty)
                return (0, new List<string> { "❌ Không tìm thấy sản phẩm với mã SKU." });

            // Tìm dòng item tương ứng
            var item = detail.OutboundTaskItems.OutboundTaskItemDetails
                .FirstOrDefault(x => x.ProductSKUId == productSkuId);

            if (item == null)
                return (0, new List<string> { "❌ SKU không khớp với sản phẩm trong đơn." });

            // Nếu đã đủ thì không cộng quá
            if (item.QuantityChecked < item.Quantity)
            {
                item.QuantityChecked++;
                if (item.QuantityChecked >= item.Quantity)
                {
                    item.QuantityChecked = item.Quantity;
                    item.IsChecked = true;
                }
                logs.Add($"✅ Đã quét: {item.QuantityChecked}/{item.Quantity}");
            }
            else
            {
                item.IsChecked = true;
                logs.Add("ℹ️ Mặt hàng này đã đủ số lượng trước đó.");
            }

            // 1) Kiểm tra tất cả item thuộc detail hiện tại
            var allItemFinished = detail.OutboundTaskItems
                .OutboundTaskItemDetails
                .All(x => x.IsChecked);

            if (allItemFinished)
            {
                detail.StatusCheckDetailTask = StatusCheckDetailTask.finished;
                detail.FinishAt = DateTime.UtcNow;
            }

            // 2) Kiểm tra toàn bộ CheckTask
            var allFinished = await _context.CheckTaskDetails
                .Where(d => d.CheckTaskId == detail.CheckTaskId)
                .AllAsync(d => d.StatusCheckDetailTask == StatusCheckDetailTask.finished);

            if (allFinished)
            {
                var checkTask = await _context.CheckTasks.FindAsync(detail.CheckTaskId);
                if (checkTask != null)
                {
                    checkTask.StatusCheckTask = StatusCheckTask.finished;
                    checkTask.FinishAt = DateTime.UtcNow;
                }
            }
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("❌ Không xác định được người dùng hiện tại.");
            await LogCheckTaskAction(detail.CheckTaskId, request.SKU, 1, $"Xác nhận kiểm hàng thủ công thành công cho NV Check #{detail.OrderIndex}", userId);

            // Lưu trước khi return
            await _context.SaveChangesAsync();

            // Ưu tiên trả về: 3 (CheckTask xong) > 2 (slot/detail xong) > 1 (chỉ xác nhận)
            if (allFinished)
            {
                logs.Add("✅ Đã xác nhận và cập nhật trạng thái.");
                return (3, logs); // Case 3
            }
            if (allItemFinished)
            {
                logs.Add("✅ Đã xác nhận và đã hoàn thành 1 slot.");
                return (2, logs); // Case 2
            }

            logs.Add("✅ Đã xác nhận.");
            return (1, logs); // Case 1
        }



        public async Task<CheckTasks> CreateCheckTaskAsync(Guid outboundTaskId)
        {
            var checkTask = new CheckTasks
            {
                Id = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow,
                MadeAt = null,
                FinishAt = null,
                StatusCheckTask = StatusCheckTask.Created,
                UsersTasksId = null,
                OutboundTaskId = outboundTaskId
            };

            _context.CheckTasks.Add(checkTask);

            var outboundTaskItems = await _context.OutboundTaskItems
                .Where(oti => oti.OutboundTaskId == outboundTaskId)
                .ToListAsync();
            // 🔽 Cập nhật trạng thái của OutboundTask
            foreach (var item in outboundTaskItems)
            {
                item.Status = StatusOutboundTaskItems.Checked;
            }
            var checkTaskDetails = outboundTaskItems
              .Select((item, index) => new CheckTaskDetails
              {
                  Id = Guid.NewGuid(),
                  CreateAt = DateTime.UtcNow,
                  StatusCheckDetailTask = StatusCheckDetailTask.Created,
                  CheckTaskId = checkTask.Id,
                  OutboundTaskItemId = item.Id,
                  OrderIndex = index + 1
              }).ToList();

            _context.CheckTaskDetails.AddRange(checkTaskDetails);
         

            await _context.SaveChangesAsync();

            return checkTask;
        }

        public async Task<CheckTaskFullViewModel> AssignUserTaskToCheckTaskByBasketAsync(Guid basketId)
        {
            var basket = await _context.Baskets.FindAsync(basketId);
            if (basket == null)
                throw new Exception("❌ Không tìm thấy giỏ hàng.");

            if (basket.OutBoundTaskId == null)
                throw new Exception("❌ Giỏ hàng chưa được gán với nhiệm vụ Outbound nào.");

            var outboundTaskId = basket.OutBoundTaskId.Value;

            var checkTask = await _context.CheckTasks
                .Include(c => c.CheckTaskDetails)
                    .ThenInclude(cd => cd.OutboundTaskItems)
                        .ThenInclude(oti => oti.OutboundTaskItemDetails)
                .FirstOrDefaultAsync(c => c.OutboundTaskId == outboundTaskId);

            if (checkTask == null)
                throw new Exception("❌ Không tìm thấy nhiệm vụ kiểm hàng (CheckTask) cho OutboundTask này.");

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("❌ Không xác định được người dùng hiện tại.");

            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
                throw new Exception("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");

            if (checkTask.UsersTasksId == null)
            {
                checkTask.UsersTasksId = userTaskId;
                checkTask.StatusCheckTask = StatusCheckTask.recived;
                await _context.SaveChangesAsync();
            }
            else if (checkTask.UsersTasksId != userTaskId)
            {
                throw new Exception("❌ Nhiệm vụ kiểm hàng này đã được người dùng khác đảm nhận.");
            }

            return new CheckTaskFullViewModel
            {
                Id = checkTask.Id,
                CreateAt = checkTask.CreateAt,
                Status = (int)checkTask.StatusCheckTask,
                UsersTasksId = checkTask.UsersTasksId,
                OutboundTaskId = checkTask.OutboundTaskId,
                Details = checkTask.CheckTaskDetails.Select(d => new CheckTaskFullViewModel.CheckTaskDetailItem
                {
                    Id = d.Id,
                    CreateAt = d.CreateAt,
                    Status = (int)d.StatusCheckDetailTask,
                    OrderIndex = d.OrderIndex,
                    OutboundTaskItem = new OutboundTaskItemVm
                    {
                        Id = d.OutboundTaskItems.Id,
                        ItemDetails = d.OutboundTaskItems.OutboundTaskItemDetails.Select(i => new OutboundTaskItemDetailVm
                        {
                            Id = i.Id,
                            ProductSKUId = i.ProductSKUId,
                            Quantity = i.Quantity
                        }).ToList()
                    }
                }).ToList()
            };
        }

        public async Task<List<string>> ValidateCheckTaskScanAsync(ScanCheckTaskRequest request)
        {
            var logs = new List<string>();

            var checkTask = await _context.CheckTasks
                .Include(ct => ct.CheckTaskDetails)
                    .ThenInclude(d => d.OutboundTaskItems)
                        .ThenInclude(oti => oti.OutboundTaskItemDetails)
                .FirstOrDefaultAsync(ct => ct.Id == request.CheckTaskId);

            if (checkTask == null)
                return new() { "❌ Không tìm thấy nhiệm vụ kiểm hàng." };

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return new() { "❌ Không xác định người dùng." };

            CheckTaskDetails? matchingDetail = null;
            foreach (var detail in checkTask.CheckTaskDetails)
            {
                if (detail.StatusCheckDetailTask != StatusCheckDetailTask.finished)
                {
                    var skuMatch = detail.OutboundTaskItems.OutboundTaskItemDetails.Any(oti =>
                        string.Equals(request.SKU, _context.ProductSKUs.FirstOrDefault(p => p.Id == oti.ProductSKUId)?.SKU,
                            StringComparison.OrdinalIgnoreCase));

                    if (skuMatch)
                    {
                        matchingDetail = detail;
                        break;
                    }
                }
            }

            if (matchingDetail == null)
            {
                await LogCheckTaskAction(request.CheckTaskId, request.SKU, 0, "❌ SKU không hợp lệ hoặc đơn hàng đã kiểm xong.", userId);
                return new() { "❌ SKU không hợp lệ hoặc đơn hàng đã kiểm xong." };
            }

            matchingDetail.QuantityChecked++;

            var totalRequired = matchingDetail.OutboundTaskItems.OutboundTaskItemDetails.Sum(x => x.Quantity);
            var totalChecked = matchingDetail.QuantityChecked;

            if (totalChecked >= totalRequired)
            {
                matchingDetail.StatusCheckDetailTask = StatusCheckDetailTask.finished;
                matchingDetail.IsChecked = true;
                logs.Add($"✅ Đã hoàn tất đơn hàng #{matchingDetail.OutboundTaskItemId}");

                await PrintOutboundBill(matchingDetail);

                var userTask = await _context.UsersTasks.FirstOrDefaultAsync(u => u.Id == checkTask.UsersTasksId);
                if (userTask != null)
                {
                    userTask.TotalKPI += totalRequired;
                    userTask.HourlyKPIs += totalRequired;
                }
                await _packTaskService.CreatePackTaskAsync(matchingDetail.OutboundTaskItemId);

            }

            if (checkTask.CheckTaskDetails.All(d => d.StatusCheckDetailTask == StatusCheckDetailTask.finished))
            {
                checkTask.StatusCheckTask = StatusCheckTask.finished;
                logs.Add("✅ Tất cả đơn hàng đã được kiểm. Nhiệm vụ hoàn tất.");
            }

            await LogCheckTaskAction(request.CheckTaskId, request.SKU, 1, null, userId);

            await _context.SaveChangesAsync();
            return logs;
        }

        private async Task LogCheckTaskAction(Guid checkTaskId, string sku, int qty, string? note, string? userId)
        {
            _context.CheckTaskLogs.Add(new CheckTaskLogs
            {
                Id = Guid.NewGuid(),
                CheckTaskId = checkTaskId,
                SKU = sku,
                QuantityChanged = qty,
                Note = note,
                PerformedBy = userId,
                Time = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
        // Method: AssignSlotAsync
        public async Task<CheckTaskResultViewModel> AssignSlotAsync(AssignSlotRequest request)
        {
            var result = new CheckTaskResultViewModel();
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                result.Success = false;
                result.Messages.Add("❌ Không xác định người dùng.");
                return result;
            }

            var checkTask = await _context.CheckTasks
                .Include(ct => ct.CheckTaskDetails)
                    .ThenInclude(d => d.OutboundTaskItems)
                        .ThenInclude(oti => oti.OutboundTaskItemDetails)
                .FirstOrDefaultAsync(ct => ct.Id == request.CheckTaskId);

            if (checkTask == null)
            {
                result.Success = false;
                result.Messages.Add("❌ Không tìm thấy nhiệm vụ kiểm hàng.");
                return result;
            }

            var pending = await _context.PendingSKU
                .Where(p => p.CheckTaskId == request.CheckTaskId && p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (pending == null)
            {
                result.Success = false;
                result.Messages.Add("❌ Không tìm thấy SKU chờ xác nhận. Hãy scan SKU trước.");
                return result;
            }

            var detail = checkTask.CheckTaskDetails
                .FirstOrDefault(d => d.OrderIndex == request.SlotIndex);

            if (detail == null)
            {
                result.Success = false;
                result.Messages.Add($"❌ Không tìm thấy đơn hàng #️{request.SlotIndex} trong nhiệm vụ.");
                return result;
            }

            var matchedSKU = detail.OutboundTaskItems.OutboundTaskItemDetails.Any(x =>
                string.Equals(pending.SKU, _context.ProductSKUs.FirstOrDefault(p => p.Id == x.ProductSKUId)?.SKU,
                    StringComparison.OrdinalIgnoreCase));

            if (!matchedSKU)
            {
                await LogCheckTaskAction(request.CheckTaskId, pending.SKU, 0, $"❌ SKU không thuộc đơn hàng #{detail.OrderIndex}.", userId);
                result.Success = false;
                result.Messages.Add("❌ SKU không thuộc đơn hàng đã chọn.");
                return result;
            }

            // ✅ Ghi nhận kiểm
            detail.QuantityChecked++;

            var required = detail.OutboundTaskItems.OutboundTaskItemDetails.Sum(x => x.Quantity);
            if (detail.QuantityChecked >= required)
            {
                detail.StatusCheckDetailTask = StatusCheckDetailTask.finished;
                detail.IsChecked = true;
                result.IsOrderCompleted = true;
                result.Messages.Add($"✅ Đơn hàng #{detail.OrderIndex} đã được kiểm xong.");

                await PrintOutboundBill(detail);

                var userTask = await _context.UsersTasks.FirstOrDefaultAsync(u => u.Id == checkTask.UsersTasksId);
                if (userTask != null)
                {
                    userTask.TotalKPI += required;
                    userTask.HourlyKPIs += required;
                }
                await _packTaskService.CreatePackTaskAsync(detail.OutboundTaskItemId);

            }

            if (checkTask.CheckTaskDetails.All(x => x.StatusCheckDetailTask == StatusCheckDetailTask.finished))
            {
                checkTask.StatusCheckTask = StatusCheckTask.finished;
                result.IsTaskCompleted = true;
                result.Messages.Add("🎉 Toàn bộ nhiệm vụ kiểm hàng đã hoàn tất.");
            }

            await LogCheckTaskAction(request.CheckTaskId, pending.SKU, 1, null, userId);
            _context.PendingSKU.Remove(pending);
            await _context.SaveChangesAsync();

            return result;
        }
        // Method: ValidateSingleSKUCheckTaskAsync
        public async Task<List<string>> ValidateSingleSKUCheckTaskAsync(SingleSKUCheckTaskRequest request)
        {
            var logs = new List<string>();
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return new() { "❌ Không xác định người dùng." };
              
            var checkTask = await _context.CheckTasks
                .Include(ct => ct.CheckTaskDetails)
                    .ThenInclude(d => d.OutboundTaskItems)
                        .ThenInclude(oti => oti.OutboundTaskItemDetails)
                .FirstOrDefaultAsync(ct => ct.Id == request.CheckTaskId);

            if (checkTask == null)
                return new() { "❌ Không tìm thấy nhiệm vụ kiểm hàng." };

            // [1] Xác minh tất cả CheckTaskDetail đều cùng 1 ProductSKUId
            var skuIds = checkTask.CheckTaskDetails
                .SelectMany(d => d.OutboundTaskItems.OutboundTaskItemDetails.Select(i => i.ProductSKUId))
                .Distinct()
                .ToList();

            if (skuIds.Count != 1)
                return new() { "❌ Nhiệm vụ có nhiều mã SKU. Vui lòng kiểm tra thủ công." };

            var expectedSKU = await _context.ProductSKUs.FindAsync(skuIds.First());
            if (expectedSKU == null || !string.Equals(expectedSKU.SKU, request.SKU, StringComparison.OrdinalIgnoreCase))
                return new() { "❌ Mã SKU không khớp với nhiệm vụ." };

            // [2] Tính tổng quantity cần kiểm
            var requiredQty = checkTask.CheckTaskDetails
                .SelectMany(d => d.OutboundTaskItems.OutboundTaskItemDetails)
                .Sum(i => i.Quantity);

            if (requiredQty != request.Quantity)
                return new() { $"❌ Số lượng không khớp. Cần kiểm: {requiredQty}, bạn nhập: {request.Quantity}" };

            // [3] Đánh dấu tất cả detail là hoàn tất
            // [3.1] Tạo PackTask tương ứng nếu chưa có
            foreach (var detail in checkTask.CheckTaskDetails)
            {
                detail.QuantityChecked = detail.OutboundTaskItems.OutboundTaskItemDetails.Sum(x => x.Quantity);
                detail.StatusCheckDetailTask = StatusCheckDetailTask.finished;
                detail.IsChecked = true;

                await PrintOutboundBill(detail);

                var outboundTaskItemId = detail.OutboundTaskItems.Id;

                var exists = await _context.PackTask.AnyAsync(p => p.OutboundTaskItemId == outboundTaskItemId);
                if (!exists)
                {
                    try
                    {
                        await _packTaskService.CreatePackTaskAsync(outboundTaskItemId); // ✅ Gọi đúng
                    }
                    catch (Exception ex)
                    {
                        logs.Add($"❌ Lỗi tạo PackTask cho {outboundTaskItemId}: {ex.Message}");
                    }
                }
            }



            // [4] Cập nhật trạng thái task nếu tất cả đã xong
            checkTask.StatusCheckTask = StatusCheckTask.finished;

            // [5] Ghi log 1 dòng
            await LogCheckTaskAction(checkTask.Id, request.SKU, request.Quantity, "✅ Xác nhận kiểm tự động (SingleSKU)", userId);

            // [6] Cộng KPI
            var userTask = await _context.UsersTasks.FirstOrDefaultAsync(u => u.Id == checkTask.UsersTasksId);
            if (userTask != null)
            {
                userTask.TotalKPI += request.Quantity;
                userTask.HourlyKPIs += request.Quantity;
            }

            // [7] Lưu thay đổi
            await _context.SaveChangesAsync();
            logs.Add("✅ Kiểm hàng thành công.");
            return logs;
        }

        private async Task PrintOutboundBill(CheckTaskDetails detail)
        {
            await Task.CompletedTask;
        }


        public async Task<IEnumerable<CheckTaskLogViewModel>> GetLogsByCheckTaskIdAsync(Guid checkTaskId)
        {
            return await _context.CheckTaskLogs
                                 .Where(x => x.CheckTaskId == checkTaskId)
                                 .OrderByDescending(x => x.Time)
                                 .Select(x => new CheckTaskLogViewModel
                                 {
                                     Time = x.Time,
                                     SKU = x.SKU,
                                     QuantityChanged = x.QuantityChanged,
                                     Note = x.Note,
                                     PerformedBy = x.PerformedBy
                                 })
                                 .ToListAsync();
        }
    }
}





/*✅ 1.Tạo nhiệm vụ kiểm hàng
➤ CreateCheckTaskAsync(Guid outboundTaskId)
Tạo CheckTask mới liên kết với OutboundTask.

Lấy tất cả OutboundTaskItem → tạo CheckTaskDetails tương ứng.

Gán OrderIndex từ #1 đến #n để người dùng có thể xác định đơn hàng dễ dàng khi kiểm.

✅ 2. Gán nhiệm vụ cho người dùng
➤ AssignUserTaskToCheckTaskByBasketAsync(Guid basketId)
Người dùng quét mã giỏ (basketId) để nhận nhiệm vụ.

Xác định CheckTask từ OutboundTaskId của giỏ.

Gán UsersTasksId vào CheckTask nếu chưa có người nhận.

Trả về CheckTaskFullViewModel chứa danh sách CheckTaskDetails, từng OutboundTaskItem và OutboundTaskItemDetails.

✅ 3. Quét SKU xác nhận kiểm hàng
➤ ValidateCheckTaskScanAsync(ScanCheckTaskRequest)
Người dùng scan mã SKU sản phẩm.

Kiểm tra SKU này thuộc đơn hàng (CheckTaskDetail) nào chưa hoàn thành.

Nếu hợp lệ → lưu tạm SKU vào bảng PendingSKU (theo UserId + CheckTaskId).

Nếu không hợp lệ → trả lỗi và log vào CheckTaskLogs.

✅ 4. Gán SKU đã quét vào đơn hàng
➤ AssignSlotAsync(AssignSlotRequest)
Người dùng scan slot #1 → #8 để xác nhận SKU vừa scan thuộc đơn hàng nào.

Nếu SKU hợp lệ:

Tăng QuantityChecked cho CheckTaskDetail.

Nếu đủ số lượng → đánh dấu IsChecked = true và StatusCheckDetailTask = Finished.

In hóa đơn đơn hàng.

Cộng KPI vào UsersTasks.

Nếu tất cả CheckTaskDetail đã xong → đánh dấu CheckTask là hoàn tất.

Xóa dòng PendingSKU sau khi xác nhận.

✅ 5. In hóa đơn
➤ PrintOutboundBill(CheckTaskDetails detail)
In đơn hàng đã hoàn tất (sử dụng máy in như Xprinter).

Dựa trên thông tin trong OutboundTaskItemDetails.

✅ 6. Ghi log hành động kiểm hàng
➤ LogCheckTaskAction(...)
Ghi lại từng thao tác:

SKU nào được kiểm

Lý do lỗi (nếu có)

Số lượng thay đổi

Người thực hiện

*/

//luồn hoạt động ValidateSingleSKUCheckTaskAsync
//✅ 1.Nhận dữ liệu đầu vào
//csharp
//Sao chép mã
//public class SingleSKUCheckTaskRequest
//{
//    public Guid CheckTaskId { get; set; }   // ID của nhiệm vụ kiểm hàng
//    public string SKU { get; set; } = null!; // Mã SKU được quét
//    public int Quantity { get; set; }       // Số lượng hàng hóa người dùng đếm được trong rổ
//}
//✅ 2.Xác minh người dùng
//Lấy UserId từ HttpContext.

//Nếu không có → báo lỗi "Không xác định người dùng."

//✅ 3. Tìm CheckTask
//Truy vấn CheckTask theo CheckTaskId, kèm theo các CheckTaskDetails, OutboundTaskItems, và OutboundTaskItemDetails.

//✅ 4. Xác minh SKU duy nhất
//Lấy danh sách ProductSKUId từ tất cả OutboundTaskItemDetails trong nhiệm vụ.

//Nếu có nhiều hơn 1 SKU → báo lỗi "Nhiệm vụ có nhiều mã SKU. Vui lòng kiểm tra thủ công."

//✅ 5. So khớp SKU
//Lấy ProductSKU từ DB theo ProductSKUId.

//So sánh với request.SKU.

//Nếu không khớp → báo lỗi "Mã SKU không khớp với nhiệm vụ."

//✅ 6. So sánh Quantity
//Tính tổng số lượng yêu cầu (requiredQty) từ tất cả OutboundTaskItemDetails.

//So sánh với request.Quantity.

//Nếu không khớp → báo lỗi "Số lượng không khớp. Cần kiểm: X, bạn nhập: Y."

//✅ 7. Cập nhật CheckTaskDetails
//Với mỗi CheckTaskDetail, thực hiện:

//Gán QuantityChecked = tổng quantity của từng OutboundTaskItem

//Gán StatusCheckDetailTask = Finished

//Gán IsChecked = true

//Gọi PrintOutboundBill(detail) để in hóa đơn.

//✅ 8. Hoàn tất CheckTask
//Gán checkTask.StatusCheckTask = Finished

//✅ 9. Ghi log 1 dòng
//csharp
//Sao chép mã
//await LogCheckTaskAction(
//    checkTask.Id,
//    request.SKU,
//    request.Quantity,
//    "✅ Xác nhận kiểm thủ công (SingleSKU)",
//    userId
//);
//✅ 10.Cộng KPI
//Cộng request.Quantity vào userTask.TotalKPI và HourlyKPIs.

//✅ 11. Lưu thay đổi và trả kết quả
//Gọi SaveChangesAsync()

//Trả kết quả "✅ Kiểm hàng thành công."

