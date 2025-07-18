using MadeHuman_Server.Data;
using MadeHuman_Server.Model.User_Task;
using Madehuman_User.ViewModel.PartTime_Task;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Service.UserTask
{
    public interface IPartTimeService
    {
        List<PartTime> GetByCompanyId(Guid companyId);
        Task<List<PartTimeViewModel>> GetAllAsync();
        Task<PartTimeViewModel> CreateAsync(PartTimeViewModel model);
        Task<PartTimeViewModel> UpdateAsync(PartTimeViewModel model);
    }
    public class PartTimeService : IPartTimeService
    {
        private readonly ApplicationDbContext _context;

        public PartTimeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PartTimeViewModel>> GetAllAsync()
        {
            return await _context.PartTimes
                .Select(x => new PartTimeViewModel
                {
                    PartTimeId = x.PartTimeId,
                    Name = x.Name,
                    CCCD = x.CCCD,
                    PhoneNumber = x.PhoneNumber,
                    StatusPartTimes = (StatusPartTimevm)x.StatusPartTimes,
                    CompanyId = x.CompanyId
                })
                .ToListAsync();
        }

        public async Task<PartTimeViewModel> CreateAsync(PartTimeViewModel model)
        {
            var entity = new PartTime
            {
                PartTimeId = Guid.NewGuid(),
                Name = model.Name,
                CCCD = model.CCCD,
                PhoneNumber = model.PhoneNumber,
                StatusPartTimes = (StatusPartTime)model.StatusPartTimes,
                CompanyId = model.CompanyId
            };

            _context.PartTimes.Add(entity);
            await _context.SaveChangesAsync();

            model.PartTimeId = entity.PartTimeId;
            return model;
        }

        public async Task<PartTimeViewModel> UpdateAsync(PartTimeViewModel model)
        {
            var entity = await _context.PartTimes.FindAsync(model.PartTimeId);
            if (entity == null) throw new Exception("PartTime not found");

            entity.Name = model.Name;
            entity.CCCD = model.CCCD;
            entity.PhoneNumber = model.PhoneNumber;
            entity.StatusPartTimes = (StatusPartTime)model.StatusPartTimes;
            entity.CompanyId = model.CompanyId;

            await _context.SaveChangesAsync();
            return model;
        }
        public List<PartTime> GetByCompanyId(Guid companyId)
        {
            return _context.PartTimes
                .Where(p => p.CompanyId == companyId)
                .ToList();
        }
    }
}
