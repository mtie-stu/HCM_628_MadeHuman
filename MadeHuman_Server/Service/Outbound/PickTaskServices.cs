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
        Task StorePickTaskDetailAsync(PickTasks task, PickTaskDetails detail, Guid userTaskId);
    }
    public class PickTaskServices : IPickTaskServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserTaskSvc _usertaskservice;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PickTaskServices(ApplicationDbContext dbContext, IUserTaskSvc userTaskSvc, IHttpContextAccessor httpContextAccessor) 
        {
            _context = dbContext;
            _usertaskservice = userTaskSvc;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PickTaskFullViewModel?> AssignPickTaskToCurrentUserAsync()
        {
            // 1. Lấy userId hiện tại
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("❌ Không xác định được người dùng hiện tại.");

            // 2. Lấy UserTaskId hôm nay của người dùng
            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
                throw new InvalidOperationException("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");

            // 3. Tìm PickTask chưa được gán
            var task = await _context.PickTasks
                .Include(p => p.PickTaskDetails)
                .Where(p => p.UsersTasksId == null || p.UsersTasksId == Guid.Empty)
                .OrderBy(p => p.CreateAt)
                .FirstOrDefaultAsync();

            if (task == null)
                return null; // 🎉 Hết PickTask chưa gán

            // 4. Gán UsersTasksId
            task.UsersTasksId = userTaskId;
            task.Status = StatusPickTask.Recived;

            await _context.SaveChangesAsync();

            // 5. Trả về ViewModel
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
            var errors = new List<string>();

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

            var sku = await _context.ProductSKUs
                .Where(p => p.Id == detail.ProductSKUId)
                .Select(p => p.SKU)
                .FirstOrDefaultAsync() ?? "(null)";

            if (!string.Equals(request.SKU, sku, StringComparison.OrdinalIgnoreCase))
                return new() { $"❌ SKU không khớp. Hệ thống: {sku}, bạn nhập: {request.SKU}" };

            await StorePickTaskDetailAsync(task, detail, userTaskId.Value);

            return new() { "✅ Đã ghi nhận 1 lần pick thành công." };
        }
        public async Task StorePickTaskDetailAsync(PickTasks task, PickTaskDetails detail, Guid userTaskId)
        {
            // Kiểm tra tồn kho
            var inventory = await _context.Inventory.FirstOrDefaultAsync(i =>
                i.ProductSKUId == detail.ProductSKUId &&
                i.WarehouseLocationId == detail.WarehouseLocationId);

            if (inventory == null || inventory.StockQuantity < 1)
                throw new Exception("❌ Không đủ tồn kho để pick. Vui lòng nhờ staff hỗ trợ điều chỉnh vị trí.");

            inventory.StockQuantity -= 1;
            inventory.LastUpdated = DateTime.UtcNow;

            detail.QuantityPicked += 1;

            _context.InventoryLogs.Add(new InventoryLogs
            {
                Id = Guid.NewGuid(),
                InventoryId = inventory.Id,
                StockQuantity = inventory.StockQuantity,
                ChangeQuantity = -1,
                RemainingQuantity = inventory.StockQuantity,
                ActionInventoryLogs = ActionInventoryLogs.Take,
                ChangeBy = userTaskId.ToString(),
                Time = DateTime.UtcNow
            });

            if (detail.QuantityPicked >= detail.Quantity)
            {
                detail.IsPicked = true;
            }

            if (task.PickTaskDetails.All(d => d.IsPicked))
            {
                task.Status = StatusPickTask.Finished;

                var totalQty = task.PickTaskDetails.Sum(d => d.Quantity);
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
