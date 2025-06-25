using MadeHuman_Server.Data;
using MadeHuman_Server.Model.User_Task;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Service.UserTask
{
    public interface IPartTimeAssignmentService
    {
        Task<PartTimeAssignment> CreateAssignmentAsync(PartTimeAssignment model);
        Task<PartTimeAssignment?> GetByIdAsync(Guid id);
        Task<IEnumerable<PartTimeAssignment>> GetAllAsync();
        Task<bool> ConfirmAssignmentAsync(Guid id);
    }
    public class PartTimeAssignmentService: IPartTimeAssignmentService
    {

        private readonly ApplicationDbContext _context;

        public PartTimeAssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PartTimeAssignment> CreateAssignmentAsync(PartTimeAssignment model)
        {
            model.Id = Guid.NewGuid();
            _context.Set<PartTimeAssignment>().Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<PartTimeAssignment?> GetByIdAsync(Guid id)
        {
            return await _context.Set<PartTimeAssignment>()
                .Include(p => p.PartTime)
                .Include(p => p.User)
                .Include(p => p.UsersTasks)
                .Include(p => p.part_Time_Company)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<PartTimeAssignment>> GetAllAsync()
        {
            return await _context.Set<PartTimeAssignment>()
                .Include(p => p.PartTime)
                .Include(p => p.User)
                .Include(p => p.UsersTasks)
                .Include(p => p.part_Time_Company)
                .ToListAsync();
        }

        public async Task<bool> ConfirmAssignmentAsync(Guid id)
        {
            var assignment = await _context.Set<PartTimeAssignment>().FindAsync(id);
            if (assignment == null) return false;

            assignment.IsConfirmed = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
