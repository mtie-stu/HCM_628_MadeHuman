using MadeHuman_Server.Data;
using MadeHuman_Server.Model.User_Task;
using Madehuman_Share.ViewModel;
using Madehuman_Share.ViewModel.PartTime_Task;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.UserTask
{
    public interface IUserTaskSvc
    {
        Task<Checkin_Checkout_Viewmodel> Checkin_Checkout_Async(Checkin_Checkout_Viewmodel model, Regime regime);
        Task<UsersTasks?> GetTodayUserTaskAsync(string userId, Guid partTimeId);
        Task AddAsync(UsersTasks userTask);
        Task UpdateAsync(UsersTasks userTask);
        Task<Guid?> GetUserTaskIdAsync(string userId, DateOnly workDate);
        Task<List<GetKPIForPartTime_Viewmodel>> GetUserTaskSummariesAsync(DateOnly workDate, TaskTypeUservm taskType);
        Task<List<CheckInCheckOutTodayViewModel>> GetCheckInOutTodayAsync();

    }

    public class UserTaskSvc : IUserTaskSvc
    {
        private readonly ApplicationDbContext _context;

        public UserTaskSvc(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<Checkin_Checkout_Viewmodel> Checkin_Checkout_Async(Checkin_Checkout_Viewmodel model, Regime regime)
        {
            if (model.PartTimeId == null)
                throw new ArgumentException("PartTimeId không được để trống.");

            var now = DateTime.UtcNow;
            var nowLocal = now.ToLocalTime(); // Hiển thị Note theo giờ địa phương (VN)

            var ptasm = await GetTodayPTAsmAsync(model.PartTimeId.Value);
            if (ptasm == null)
                throw new InvalidOperationException("Không có ca làm việc nào được phân công cho hôm nay.");

            var userTask = await GetTodayUserTaskAsync(model.UserId, model.PartTimeId.Value);

            switch (regime)
            {
                case Regime.Checkin:
                    if (userTask == null)
                    {
                        // ✅ Checkin lần đầu
                        userTask = new UsersTasks
                        {
                            Id = Guid.NewGuid(),
                            UserId = model.UserId,
                            PartTimeId = model.PartTimeId.Value,
                            WorkDate = now,
                            CheckInTime = now,
                            Note = string.IsNullOrWhiteSpace(model.Note)
                                ? $"Đã checkin lúc {nowLocal:HH:mm dd/MM/yyyy}"
                                : model.Note,
                            TaskType = (TaskTypeUser)ptasm.TaskType,
                            IsCompleted = false
                        };

                        await AddAsync(userTask);
                        ptasm.UsersTasksId = userTask.Id;

                        _context.CheckInCheckOutLog.Add(new CheckInCheckOutLog
                        {
                            Id = Guid.NewGuid(),
                            UserId = model.UserId,
                            PartTimeId = model.PartTimeId.ToString(),
                            Timestamp = now,
                            IsCheckIn = true,
                            IsOvertime = false,
                            Note = $"Đã checkin vào kho lúc {nowLocal:HH:mm dd/MM/yyyy}",
                            UsersTasksId = userTask.Id
                        });
                    }
                    else if (userTask.CheckOutTime.HasValue)
                    {
                        // ✅ Bắt đầu tăng ca
                        _context.CheckInCheckOutLog.Add(new CheckInCheckOutLog
                        {
                            Id = Guid.NewGuid(),
                            UserId = model.UserId,
                            PartTimeId = model.PartTimeId.ToString(),
                            Timestamp = now,
                            IsCheckIn = true,
                            IsOvertime = true,
                            Note = $"Đã bắt đầu tăng ca lúc {nowLocal:HH:mm dd/MM/yyyy}",
                            UsersTasksId = userTask.Id
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException("Đã checkin và chưa checkout.");
                    }
                    break;

                case Regime.Checkout:
                    if (userTask == null)
                        throw new InvalidOperationException("Chưa checkin hôm nay.");

                    if (!userTask.CheckOutTime.HasValue)
                    {
                        // ✅ Checkout lần đầu
                        userTask.CheckOutTime = now;
                        userTask.Note = string.IsNullOrWhiteSpace(model.Note)
                            ? $"Đã checkout lúc {nowLocal:HH:mm dd/MM/yyyy}"
                            : model.Note;
                        userTask.IsCompleted = true;

                        await UpdateAsync(userTask);

                        _context.CheckInCheckOutLog.Add(new CheckInCheckOutLog
                        {
                            Id = Guid.NewGuid(),
                            UserId = model.UserId,
                            PartTimeId = model.PartTimeId.ToString(),
                            Timestamp = now,
                            IsCheckIn = false,
                            IsOvertime = false,
                            Note = $"Đã checkout rời khỏi kho hàng lúc {nowLocal:HH:mm dd/MM/yyyy}",
                            UsersTasksId = userTask.Id
                        });
                    }
                    else
                    {
                        // ✅ Kết thúc tăng ca
                        var lastOvertimeCheckin = await _context.CheckInCheckOutLog
                            .Where(x =>
                                x.UserId == model.UserId &&
                                x.PartTimeId == model.PartTimeId.ToString() &&
                                x.IsCheckIn == true &&
                                x.IsOvertime == true)
                            .OrderByDescending(x => x.Timestamp)
                            .FirstOrDefaultAsync();

                        var hasCheckout = await _context.CheckInCheckOutLog.AnyAsync(x =>
                            x.UserId == model.UserId &&
                            x.PartTimeId == model.PartTimeId.ToString() &&
                            x.IsCheckIn == false &&
                            x.IsOvertime == true &&
                            x.Timestamp > lastOvertimeCheckin.Timestamp);

                        if (lastOvertimeCheckin == null || hasCheckout)
                            throw new InvalidOperationException("Không tìm thấy phiên tăng ca nào cần kết thúc.");

                        var overtimeDuration = now - lastOvertimeCheckin.Timestamp;

                        userTask.OvertimeDuration = (userTask.OvertimeDuration ?? TimeSpan.Zero) + overtimeDuration;
                        ptasm.OvertimeDuration = (ptasm.OvertimeDuration ?? TimeSpan.Zero) + overtimeDuration;

                        _context.CheckInCheckOutLog.Add(new CheckInCheckOutLog
                        {
                            Id = Guid.NewGuid(),
                            UserId = model.UserId,
                            PartTimeId = model.PartTimeId.ToString(),
                            Timestamp = now,
                            IsCheckIn = false,
                            IsOvertime = true,
                            Note = $"Đã kết thúc tăng ca lúc {nowLocal:HH:mm dd/MM/yyyy}",
                            UsersTasksId = userTask.Id
                        });
                    }
                    break;

                case Regime.Break:
                    if (userTask == null)
                        throw new InvalidOperationException("Chưa checkin hôm nay.");

                    userTask.BreakDuration = model.BreakDuration ?? TimeSpan.Zero;
                    userTask.Note = string.IsNullOrWhiteSpace(model.Note)
                        ? $"Đã bắt đầu nghỉ giải lao: {userTask.BreakDuration.Value.TotalMinutes} phút"
                        : model.Note;

                    await UpdateAsync(userTask);

                    _context.CheckInCheckOutLog.Add(new CheckInCheckOutLog
                    {
                        Id = Guid.NewGuid(),
                        UserId = model.UserId,
                        PartTimeId = model.PartTimeId.ToString(),
                        Timestamp = now,
                        IsCheckIn = false,
                        IsOvertime = false,
                        Note = $"Đã bắt đầu ra nghỉ giải lao lúc {nowLocal:HH:mm dd/MM/yyyy}",
                        UsersTasksId = userTask.Id
                    });
                    break;

                default:
                    throw new NotSupportedException("Regime không được hỗ trợ.");
            }

            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<UsersTasks?> GetTodayUserTaskAsync(string userId, Guid partTimeId)
        {
            // Chuyển DateTime.Today về UTC để phù hợp với PostgreSQL timestamp with time zone
            var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);

            return await _context.UsersTasks
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.PartTimeId == partTimeId &&
                    x.CheckInTime.HasValue &&
                    x.CheckInTime.Value.Date == today);
        }

        public async Task<PartTimeAssignment?> GetTodayPTAsmAsync(Guid partTimeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.PartTimeAssignment
                .FirstOrDefaultAsync(x =>
                    x.PartTimeId == partTimeId &&
                    x.WorkDate == today);
        }

        public async Task<Guid?> GetUserTaskIdAsync(string userId, DateOnly workDate)
        {
            var date = DateTime.SpecifyKind(workDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var nextDate = date.AddDays(1); // Vì date đã là UTC nên nextDate cũng sẽ là UTC

            var userTask = await _context.UsersTasks
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.WorkDate >= date && x.WorkDate < nextDate);

            return userTask?.Id;
        }

        public async Task<List<GetKPIForPartTime_Viewmodel>> GetUserTaskSummariesAsync(DateOnly workDate, TaskTypeUservm taskType)
        {
            var targetDate = DateTime.SpecifyKind(workDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var taskTypeEntity = (TaskTypeUser)taskType;

            var result = await _context.UsersTasks
                .Where(x => x.WorkDate.Date == targetDate.Date && x.TaskType == taskTypeEntity)
                .Include(x => x.User)
                .Select(x => new GetKPIForPartTime_Viewmodel
                {
                    TaskType = (TaskTypeUservm)x.TaskType,
                    WorkDate = x.WorkDate,
                    Email = x.User.Email,
                    PartTimeId = x.PartTimeId,
                    TotalKPI = x.TotalKPI,
                    HourlyKPIs = x.HourlyKPIs
                })
                .ToListAsync();

            return result;
        }


        public async Task AddAsync(UsersTasks userTask)
        {
            await _context.UsersTasks.AddAsync(userTask);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UsersTasks userTask)
        {
            _context.UsersTasks.Update(userTask);
            await _context.SaveChangesAsync();
        }
        public async Task<List<CheckInCheckOutTodayViewModel>> GetCheckInOutTodayAsync()
        {
            var today = DateTime.UtcNow.Date;

            var logs = await _context.CheckInCheckOutLog
                .Where(x => x.Timestamp.Date == today)
                .OrderBy(x => x.Timestamp)
                .Select(x => new CheckInCheckOutTodayViewModel
                {
                    UserId = x.UserId,
                    PartTimeId = x.PartTimeId,
                    Timestamp = x.Timestamp,
                    IsCheckIn = x.IsCheckIn,
                    IsOvertime = x.IsOvertime,
                    Note = x.Note
                })
                .ToListAsync();

            return logs;
        }
    }
}
