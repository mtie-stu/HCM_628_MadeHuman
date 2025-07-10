using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Inbound;
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
            var task = new RefillTasks
            {
                Id = Guid.NewGuid(),
                 StatusRefillTasks=StatusRefillTasks.Incomplete,
                CreateAt = DateTime.UtcNow,
                CreateBy = UserId,
                RefillTaskDetails = vm.Details.Select(d => new RefillTaskDetails
                {
                    Id = Guid.NewGuid(),
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

    }
}
