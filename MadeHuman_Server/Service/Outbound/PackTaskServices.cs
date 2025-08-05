using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Service.UserTask;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IPackTaskService
    {
        Task<PackTask> CreatePackTaskAsync(Guid outboundTaskItemId);
        Task<List<string>> AssignPackTaskAsync(Guid outboundTaskItemId);
    }
    public class PackTaskServices : IPackTaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserTaskSvc _userTaskService;
        private readonly IDispatchTaskServices _dispatchTaskServices;   

        public PackTaskServices(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IUserTaskSvc userTaskService,IDispatchTaskServices dispatchTaskServices)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userTaskService = userTaskService;
            _dispatchTaskServices = dispatchTaskServices;
        }

        public async Task<PackTask> CreatePackTaskAsync(Guid outboundTaskItemId)
        {
            // Kiểm tra OutboundTaskItem tồn tại
            var outboundItem = await _context.OutboundTaskItems.FindAsync(outboundTaskItemId);
            if (outboundItem == null)
                throw new Exception("OutboundTaskItem không tồn tại.");

           
            var packTask = new PackTask
            {
                Id = Guid.NewGuid(),
                StatusPackTask = StatusPackTask.Created,
                OutboundTaskItemId = outboundTaskItemId,
                OutboundTaskItems = outboundItem // ✅ thêm dòng này
            };

            _context.PackTask.Add(packTask);
            await _context.SaveChangesAsync();

            return packTask;
        }


        public async Task<List<string>> AssignPackTaskAsync(Guid outboundTaskItemId)
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

            var packTask = await _context.PackTask
            .Include(p => p.OutboundTaskItems)
                .ThenInclude(oti => oti.OutboundTask)
            .FirstOrDefaultAsync(p => p.OutboundTaskItemId == outboundTaskItemId);

            if (packTask?.OutboundTaskItems?.OutboundTask == null)
            {
                logs.Add("❌ Không tìm thấy OutboundTask.");
                return logs;
            }

            var outboundTaskId = packTask.OutboundTaskItems.OutboundTask.Id;
            logs.Add($"📦 OutboundTaskId là: {outboundTaskId}");


            if (packTask == null)
            {
                logs.Add("❌ Không tìm thấy PackTask.");
                return logs;
            }

            if (packTask.UsersTasksId == null)
            {
                packTask.UsersTasksId = userTaskId;
                packTask.StatusPackTask = StatusPackTask.Recived;
                logs.Add("✅ Gán người dùng vào PackTask thành công.");
            }
            else if (packTask.UsersTasksId != userTaskId)
            {
                logs.Add("❌ PackTask này đã được người khác đảm nhận.");
                return logs;
            }

            // Cộng KPI
            var userTask = await _context.UsersTasks.FirstOrDefaultAsync(u => u.Id == userTaskId);
            if (userTask != null)
            {
                userTask.TotalKPI += 1;
                userTask.HourlyKPIs += 1;
            }
            packTask.StatusPackTask= StatusPackTask.Finished;
            packTask.FinishAt = DateTime.UtcNow;
            await _dispatchTaskServices.CreateDisPactchPackTaskAsync(outboundTaskItemId);
            await _context.SaveChangesAsync();
            logs.Add("✅ Đã cộng KPI cho người dùng.");
            return logs;
        }

    }
}