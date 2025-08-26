using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Service.UserTask;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IDispatchTaskServices
    {
        Task<DispatchTasks> CreateDisPactchPackTaskAsync(Guid outboundTaskItemId);
        Task<List<string>> AssignDisPactchTaskAsync(Guid outboundTaskItemId);
    }
    public class DispatchTaskServices : IDispatchTaskServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserTaskSvc _userTaskService;

        public DispatchTaskServices(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IUserTaskSvc userTaskService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userTaskService = userTaskService;
        }

        public async Task<DispatchTasks> CreateDisPactchPackTaskAsync(Guid outboundTaskItemId)
        {
            // Kiểm tra OutboundTaskItem tồn tại
            var outboundItem = await _context.OutboundTaskItems.FindAsync(outboundTaskItemId);
            if (outboundItem == null)
                throw new Exception("OutboundTaskItem không tồn tại.");


            var dispatchTasks = new DispatchTasks
            {
                Id = Guid.NewGuid(),
                StatusDispatchTasks = StatusDispatchTasks.Created,
                OutboundTaskItemId = outboundTaskItemId
            };
            outboundItem.Status = StatusOutboundTaskItems.Dispatched;
            _context.DispatchTasks.Add(dispatchTasks);
            await _context.SaveChangesAsync();

            return dispatchTasks;
        }
        public async Task<List<string>> AssignDisPactchTaskAsync(Guid outboundTaskItemId)
        {
            var logs = new List<string>();

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logs.Add("❌ Không xác định người dùng.");
                return logs;
            }

            var userTaskId = await _userTaskService.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
            {
                logs.Add("❌ Người dùng chưa có phân công công việc hôm nay.");
                return logs;
            }

            var dispatchTasks = await _context.DispatchTasks
                .Include(p => p.OutboundTaskItems)
                .FirstOrDefaultAsync(p => p.OutboundTaskItemId == outboundTaskItemId);

            if (dispatchTasks == null)
            {
                logs.Add("❌ Không tìm thấy DispatchTask.");
                return logs;
            }

            if (dispatchTasks.UsersTasksId == null)
            {
                dispatchTasks.UsersTasksId = userTaskId;
                dispatchTasks.StatusDispatchTasks = StatusDispatchTasks.Recived;
                logs.Add("✅ Gán người dùng vào DispatchTask thành công.");
            }
            else if (dispatchTasks.UsersTasksId != userTaskId)
            {
                logs.Add("❌ DispatchTask này đã được người khác đảm nhận.");
                return logs;
            }

            // Cộng KPI
            var userTask = await _context.UsersTasks.FirstOrDefaultAsync(u => u.Id == userTaskId);
            if (userTask != null)
            {
                userTask.TotalKPI += 1;
                userTask.HourlyKPIs += 1;
            }

            dispatchTasks.StatusDispatchTasks = StatusDispatchTasks.Finished;
            dispatchTasks.FinishAt = DateTime.UtcNow;

            // ✅ Cập nhật trạng thái item trước khi kiểm tra
            var outboundItem = await _context.OutboundTaskItems
                .Include(oti => oti.OutboundTask)
                .FirstOrDefaultAsync(oti => oti.Id == outboundTaskItemId);

            if (outboundItem == null)
            {
                logs.Add("❌ Không tìm thấy OutboundTaskItem.");
                return logs;
            }

            outboundItem.Status = StatusOutboundTaskItems.Finished;

            // ✅ Kiểm tra và cập nhật trạng thái OutboundTask nếu tất cả item đã hoàn thành
            var outboundTaskId = outboundItem.OutboundTaskId;

            var allItems = await _context.OutboundTaskItems
                .Where(i => i.OutboundTaskId == outboundTaskId)
                .ToListAsync();

            // Lưu ý: trạng thái của `outboundItem` đã cập nhật trong context (chưa save)
            if (allItems.All(i => i.Id == outboundTaskItemId ?
                                  StatusOutboundTaskItems.Finished == StatusOutboundTaskItems.Finished :
                                  i.Status == StatusOutboundTaskItems.Finished))
            {
                var outboundTask = await _context.OutboundTasks.FindAsync(outboundTaskId);
                if (outboundTask != null)
                {
                    outboundTask.Status = StatusOutbountTask.Completed;
                    logs.Add("✅ OutboundTask đã hoàn thành tất cả item, cập nhật trạng thái thành Finished.");
                }
            }

            await _context.SaveChangesAsync();
            logs.Add("✅ Đã cộng KPI cho người dùng ");
            return logs;
        }

    }
}
