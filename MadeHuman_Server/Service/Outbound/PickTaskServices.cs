using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IPickTaskServices
    {
        Task<PickTaskFullViewModel?> AssignPickTaskToCurrentUserAsync();
        Task<List<string>> ValidatePickTaskScanAsync(ScanPickTaskValidationRequest request);
        Task StorePickTaskDetailAsync(PickTasks task, PickTaskDetails detail, Guid userTaskId, Guid basketId);
    }

    public class PickTaskServices : IPickTaskServices
    {
        private readonly ICheckTaskServices _checkTaskService;

        private readonly ApplicationDbContext _context;
        private readonly IUserTaskSvc _usertaskservice;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PickTaskServices(ApplicationDbContext dbContext, IUserTaskSvc userTaskSvc, IHttpContextAccessor httpContextAccessor, ICheckTaskServices checkTaskService)
        {
            _context = dbContext;
            _usertaskservice = userTaskSvc;
            _httpContextAccessor = httpContextAccessor;
            _checkTaskService = checkTaskService;
        }

        public async Task<PickTaskFullViewModel?> AssignPickTaskToCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("❌ Không xác định được người dùng hiện tại.");

            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
                throw new InvalidOperationException("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");

            var task = await _context.PickTasks
                .Include(p => p.PickTaskDetails)
                .Where(p => p.UsersTasksId == null || p.UsersTasksId == Guid.Empty)
                .OrderBy(p => p.CreateAt)
                .FirstOrDefaultAsync();

            if (task == null)
                return null;

            task.UsersTasksId = userTaskId;
            task.Status = StatusPickTask.Recived;
            await _context.SaveChangesAsync();

            return new PickTaskFullViewModel
            {
                Id = task.Id,
                CreateAt = task.CreateAt,
                FinishAt = task.FinishAt,
                Status = (StatusPickTaskvm)task.Status,
                UsersTasksId = task.UsersTasksId,
                Details = task.PickTaskDetails.Select(d => new PickTaskFullViewModel.PickTaskDetailItem
                {
                    Id = d.Id,
                    Quantity = d.Quantity,
                    ProductSKUId = d.ProductSKUId,
                    WarehouseLocationId = d.WarehouseLocationId
                }).ToList()
            };
        }

        public async Task<List<string>> ValidatePickTaskScanAsync(ScanPickTaskValidationRequest request)
        {
            var task = await _context.PickTasks
                .Include(t => t.PickTaskDetails)
                .FirstOrDefaultAsync(t => t.Id == request.PickTaskId);

            if (task == null)
                return new() { "❌ Không tìm thấy PickTask." };

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return new() { "❌ Không xác định được người dùng." };

            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
                return new() { "❌ Không tìm thấy phân công công việc hôm nay." };

            if (task.UsersTasksId != userTaskId)
                return new() { "❌ PickTask này không được gán cho bạn." };

            var detail = task.PickTaskDetails.FirstOrDefault(d => d.Id == request.PickTaskDetailId);
            if (detail == null)
                return new() { "❌ Không tìm thấy chi tiết Pick." };

            if (detail.IsPicked)
                return new() { "❌ Chi tiết này đã hoàn tất." };
            // ✅ Gán OutboundTaskId vào basket
            var assignResult = await AssignBasketToOutboundTaskAsync(request.BasketId.Value, task.OutboundTaskId);
            if (assignResult.Any())
                return assignResult;


            var sku = await _context.ProductSKUs
                .Where(p => p.Id == detail.ProductSKUId)
                .Select(p => p.SKU)
                .FirstOrDefaultAsync() ?? "(null)";

            if (!string.Equals(request.SKU, sku, StringComparison.OrdinalIgnoreCase))
                return new() { $"❌ SKU không khớp. Hệ thống: {sku}, bạn nhập: {request.SKU}" };

            if (request.BasketId == null)
                return new() { "❌ Bạn chưa quét giỏ." };

         
            // ✅ Ghi nhận pick
            await StorePickTaskDetailAsync(task, detail, userTaskId.Value, request.BasketId.Value);

            return new() { "✅ Đã ghi nhận 1 lần pick thành công." };
        }

        public async Task StorePickTaskDetailAsync(PickTasks task, PickTaskDetails detail, Guid userTaskId, Guid basketId)
        {
            detail.QuantityPicked += 1;

            if (detail.QuantityPicked < detail.Quantity)
            {
                await _context.SaveChangesAsync();
                return;
            }

            detail.IsPicked = true;

            // ✅ Kiểm tra giỏ có khớp với OutboundTask hay không
            var basket = await _context.Baskets.FindAsync(basketId);
            if (basket == null)
                throw new Exception("❌ Không tìm thấy giỏ hàng đã quét.");

            if (basket.OutBoundTaskId != task.OutboundTaskId)
                throw new Exception("❌ Giỏ hàng không khớp với nhiệm vụ pick hiện tại.");

            // ✅ Trừ tồn kho và ghi log
            var inventory = await _context.Inventory.FirstOrDefaultAsync(i =>
                i.ProductSKUId == detail.ProductSKUId &&
                i.WarehouseLocationId == detail.WarehouseLocationId);

            if (inventory == null || inventory.StockQuantity < detail.Quantity)
                throw new Exception("❌ Không đủ tồn kho để hoàn tất pick. Vui lòng nhờ staff hỗ trợ.");

            inventory.StockQuantity -= detail.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;

            _context.InventoryLogs.Add(new InventoryLogs
            {
                Id = Guid.NewGuid(),
                InventoryId = inventory.Id,
                StockQuantity = inventory.StockQuantity,
                ChangeQuantity = -detail.Quantity,
                RemainingQuantity = inventory.StockQuantity,
                ActionInventoryLogs = ActionInventoryLogs.Take,
                ChangeBy = userTaskId.ToString(),
                Time = DateTime.UtcNow
            });

            if (task.PickTaskDetails.All(d => d.IsPicked))
            {
                task.Status = StatusPickTask.Finished;
                task.FinishAt = DateTime.UtcNow;

                var totalQty = task.PickTaskDetails.Sum(d => d.Quantity);
                var userTask = await _context.UsersTasks.FirstOrDefaultAsync(u => u.Id == userTaskId);
                if (userTask != null)
                {
                    userTask.TotalKPI += totalQty;
                    userTask.HourlyKPIs += totalQty;
                }
                // ✅ Gọi tạo CheckTask khi Pick hoàn tất
                await _checkTaskService.CreateCheckTaskAsync(task.OutboundTaskId);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<string>> AssignBasketToOutboundTaskAsync(Guid basketId, Guid outboundTaskId)
        {
            var basket = await _context.Baskets.FindAsync(basketId);
            if (basket == null)
                return new() { "❌ Không tìm thấy giỏ." };

            if (basket.OutBoundTaskId != null && basket.OutBoundTaskId != outboundTaskId)
                return new() { "❌ Giỏ này đã được sử dụng cho nhiệm vụ khác. Vui lòng quét giỏ khác." };

            if (basket.OutBoundTaskId == null)
            {
                basket.OutBoundTaskId = outboundTaskId;
                basket.Status = StatusBaskets.Selected;
                _context.Baskets.Update(basket);
                await _context.SaveChangesAsync();
            }

            return new(); // OK
        }
    }
}
