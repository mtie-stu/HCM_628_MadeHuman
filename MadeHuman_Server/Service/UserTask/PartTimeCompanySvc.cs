using MadeHuman_Server.Data;
using MadeHuman_Server.Model.User_Task;
using Madehuman_User.ViewModel.PartTime_Task;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Service.UserTask
{
    public interface IPartTimeCompanyService
    {
        Task<List<PartTimeCompanyViewModel>> GetAllAsync();
        Task<PartTimeCompanyViewModel> CreateAsync(PartTimeCompanyViewModel model);
        Task<PartTimeCompanyViewModel> UpdateAsync(PartTimeCompanyViewModel model);
    }
    public class PartTimeCompanySvc : IPartTimeCompanyService
    {
        private readonly ApplicationDbContext _context;

        public PartTimeCompanySvc(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PartTimeCompanyViewModel>> GetAllAsync()
        {
            return await _context.PartTimeCompanies
                .Select(x => new PartTimeCompanyViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Status = (StatusPart_Time_Companyvm)x.Status
                })
                .ToListAsync();
        }

        public async Task<PartTimeCompanyViewModel> CreateAsync(PartTimeCompanyViewModel model)
        {
            var entity = new Part_Time_Company
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Address = model.Address,
                Status = (StatusPart_Time_Company)model.Status
            };

            _context.PartTimeCompanies.Add(entity);
            await _context.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<PartTimeCompanyViewModel> UpdateAsync(PartTimeCompanyViewModel model)
        {
            var entity = await _context.PartTimeCompanies.FindAsync(model.Id);
            if (entity == null) throw new Exception("Company not found");

            entity.Name = model.Name;
            entity.Address = model.Address;
            entity.Status = (StatusPart_Time_Company)model.Status;

            await _context.SaveChangesAsync();
            return model;
        }
    }
}