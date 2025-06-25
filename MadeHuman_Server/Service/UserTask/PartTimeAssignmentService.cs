/*using MadeHuman_Server.Data;
using MadeHuman_Server.Model.User_Task;
using Madehuman_Share.ViewModel.PartTime_Task;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Service.UserTask
{
    public interface IPartTimeAssignmentService
    {
        Task<List<PartTimeAssignmentViewModel>> GetAllAsync();
        Task<PartTimeAssignmentViewModel> CreateAsync(PartTimeAssignmentViewModel model);
        Task<PartTimeAssignmentViewModel> UpdateAsync(PartTimeAssignmentViewModel model);
    }
    public class PartTimeAssignmentService : IPartTimeAssignmentService
    {

        private readonly ApplicationDbContext _context;

        public PartTimeAssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PartTimeAssignmentViewModel>> GetAllAsync()
        {
            return await _context.PartTimeAssignment
                .Select(x => new PartTimeAssignmentViewModel
                {
                    Id = x.Id,
                    PartTimeId = x.PartTimeId,
                    WorkDate = x.WorkDate,
                    TaskType = (TaskTypevm)x.TaskType,
                    ShiftCode = x.ShiftCode,
                    IsConfirmed = x.IsConfirmed,
                    CheckInTime = x.CheckInTime,
                    CheckOutTime = x.CheckOutTime,
                    BreakDuration = x.BreakDuration,
                    Note = x.Note,
                    UserId = x.UserId,
                    UsersTasksId = x.UsersTasksId,
                    CompanyId = x.CompanyId
                })
                .ToListAsync();
        }

        public async Task<PartTimeAssignmentViewModel> CreateAsync(PartTimeAssignmentViewModel model)
        {
            var entity = new PartTimeAssignment
            {
                Id = Guid.NewGuid(),
                PartTimeId = model.PartTimeId,
                WorkDate = model.WorkDate,
                TaskType = (TaskType)model.TaskType,
                ShiftCode = model.ShiftCode,
                IsConfirmed = model.IsConfirmed,
                CheckInTime = model.CheckInTime,
                CheckOutTime = model.CheckOutTime,
                BreakDuration = model.BreakDuration,
                Note = model.Note,
                UserId = model.UserId,
                UsersTasksId = model.UsersTasksId,
                CompanyId = model.CompanyId
            };

            _context.PartTimeAssignment.Add(entity);
            await _context.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<PartTimeAssignmentViewModel> UpdateAsync(PartTimeAssignmentViewModel model)
        {
            var entity = await _context.PartTimeAssignment.FindAsync(model.Id);
            if (entity == null) throw new Exception("Assignment not found");

            entity.PartTimeId = model.PartTimeId;
            entity.WorkDate = model.WorkDate;
            entity.TaskType = (TaskType)model.TaskType;
            entity.ShiftCode = model.ShiftCode;
            entity.IsConfirmed = model.IsConfirmed;
            entity.CheckInTime = model.CheckInTime;
            entity.CheckOutTime = model.CheckOutTime;
            entity.BreakDuration = model.BreakDuration;
            entity.Note = model.Note;
            entity.UserId = model.UserId;
            entity.UsersTasksId = model.UsersTasksId;
            entity.CompanyId = model.CompanyId;

            await _context.SaveChangesAsync();
            return model;
        }
    }
}
*/