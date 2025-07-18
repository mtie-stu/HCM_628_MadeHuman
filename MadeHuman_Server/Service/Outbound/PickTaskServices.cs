using MadeHuman_Server.Data;
using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IPickTaskServices
    {
      Task<PickTaskFullViewModel?> AssignPickTaskToCurrentUserAsync();

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
            task.Status = (Model.Outbound.StatusPickTask)StatusPickTask.Recived;

            await _context.SaveChangesAsync();

            // 5. Trả về ViewModel
            return new PickTaskFullViewModel
            {
                Id = task.Id,
                CreateAt = task.CreateAt,
                FinishAt = task.FinishAt,
                Status = (StatusPickTask)task.Status,
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

    }
}
