using MadeHuman_Server.Data;
//using MadeHuman_Server.Migrations;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.Outbound;
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
_context = context;
_usertaskservice = userTaskSvc;
_httpContextAccessor = httpContextAccessor;

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

        public async Task<CheckTaskFullViewModel> AssignUserTaskToCheckTaskByBasketAsync(Guid basketId)
        {
            // 1. Lấy Basket và OutboundTaskId
            var basket = await _context.Baskets.FindAsync(basketId);
            if (basket == null)
                throw new Exception("❌ Không tìm thấy giỏ hàng.");

            if (basket.OutBoundTaskId == null)
                throw new Exception("❌ Giỏ hàng chưa được gán với nhiệm vụ Outbound nào.");

            var outboundTaskId = basket.OutBoundTaskId.Value;

            // 2. Lấy CheckTask bao gồm: CheckDetails → OutboundTaskItem → OutboundTaskItemDetails
            var checkTask = await _context.CheckTasks
                .Include(c => c.CheckTaskDetails)
                    .ThenInclude(cd => cd.OutboundTaskItems)
                        .ThenInclude(oti => oti.OutboundTaskItemDetails)
                .FirstOrDefaultAsync(c => c.OutboundTaskId == outboundTaskId);

            if (checkTask == null)
                throw new Exception("❌ Không tìm thấy nhiệm vụ kiểm hàng (CheckTask) cho OutboundTask này.");

            // 3. Lấy UserId hiện tại
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("❌ Không xác định được người dùng hiện tại.");

            // 4. Lấy UserTaskId
            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
                throw new Exception("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");

            // 5. Gán UserTaskId nếu chưa ai nhận
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

            // 6. Mapping ViewModel trả về
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





    }
}
