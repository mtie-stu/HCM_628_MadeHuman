using MadeHuman_Server.Data;
using MadeHuman_Server.Migrations;
using MadeHuman_Server.Model.Outbound;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Outbound
{
    public interface ICheckTaskServices
    {
        Task<CheckTasks> CreateCheckTaskAsync(Guid outboundTaskId);

    }

    public class CheckTaskServices
    {
        private readonly ApplicationDbContext _context; 
        public async Task<CheckTasks> CreateCheckTaskAsync(Guid outboundTaskId)
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



    }
}
