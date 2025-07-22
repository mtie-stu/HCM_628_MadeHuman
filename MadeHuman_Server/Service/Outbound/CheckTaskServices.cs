using MadeHuman_Server.Data;
//using MadeHuman_Server.Migrations;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Service.UserTask;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Outbound
{
    public interface ICheckTaskServices
    {
        Task<CheckTasks> CreateCheckTaskAsync(Guid outboundTaskId);
        Task<List<string>> AssignUserTaskToCheckTaskByBasketAsync(Guid basketId);
    }

    public class CheckTaskServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserTaskSvc _usertaskservice;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckTaskServices(ApplicationDbContext context, IUserTaskSvc userTaskSvc, IHttpContextAccessor httpContextAccessor)
        {
             context= _context;
            userTaskSvc = _usertaskservice;
            httpContextAccessor = _httpContextAccessor;
        }
        public async Task<CheckTasks> CreateCheckTaskAsync(Guid outboundTaskId  )
        {
            // 1. Tạo CheckTask mới
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

            // 2. Truy vấn các OutboundTaskItem theo outboundTaskId
            var outboundTaskItems = await _context.OutboundTaskItems
                .Where(oti => oti.OutboundTaskId == outboundTaskId)
                .ToListAsync();

            // 3. Tạo các CheckTaskDetails tương ứng
            var checkTaskDetails = outboundTaskItems.Select(item => new CheckTaskDetails
            {
                Id = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow,
                StatusCheckDetailTask = StatusCheckDetailTask.Created,
                CheckTaskId = checkTask.Id,
                OutboundTaskItemId = item.Id
            }).ToList();

            _context.CheckTaskDetails.AddRange(checkTaskDetails);

            // 4. Lưu thay đổi
            await _context.SaveChangesAsync();

            return checkTask;
        }

        public async Task<List<string>> AssignUserTaskToCheckTaskByBasketAsync(Guid basketId)
        {
            // 1. Lấy Basket
            var basket = await _context.Baskets.FindAsync(basketId);
            if (basket == null)
                return new() { "❌ Không tìm thấy giỏ hàng." };

            if (basket.OutBoundTaskId == null)
                return new() { "❌ Giỏ hàng chưa được gán với nhiệm vụ Outbound nào." };

            var outboundTaskId = basket.OutBoundTaskId.Value;

            // 2. Tìm CheckTask dựa trên OutboundTaskId
            var checkTask = await _context.CheckTasks
                .FirstOrDefaultAsync(c => c.OutboundTaskId == outboundTaskId);

            if (checkTask == null)
                return new() { "❌ Không tìm thấy nhiệm vụ kiểm hàng (CheckTask) cho OutboundTask này." };
            if (checkTask.StatusCheckTask == StatusCheckTask.recived || checkTask.StatusCheckTask == StatusCheckTask.finished)
                return new() { "❌ Không tìm thấy nhiệm vụ kiểm hàng (CheckTask) cho OutboundTask này." };
            // 3. Lấy UserId từ HttpContext
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return new() { "❌ Không xác định được người dùng hiện tại." };

            // 4. Lấy UserTaskId từ service
            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
                return new() { "❌ Không tìm thấy phân công công việc hôm nay cho người dùng." };

            // 5. Gán UserTaskId vào CheckTask
            checkTask.UsersTasksId = userTaskId;
            checkTask.StatusCheckTask = StatusCheckTask.recived; // (nếu bạn có enum này)
            await _context.SaveChangesAsync();

            return new() { "✅ Đã gán CheckTask cho người dùng thành công." };
        }


    }
}
