using MadeHuman_Server.Data;
using MadeHuman_Server.Model.User_Task;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Service.UserTask
{
    public interface IPartTimeCompanyService
    {
        Task<Part_Time_Company> AddCompanyAsync(string name, string? address);
        Task<bool> SetActiveAsync(Guid companyId);
    }
    public class PartTimeCompanySvc : IPartTimeCompanyService
    {
        private readonly ApplicationDbContext _context;

        public PartTimeCompanySvc(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Part_Time_Company> AddCompanyAsync(string name, string? address)
        {
            var company = new Part_Time_Company
            {
                Id = Guid.NewGuid(),
                Name = name,
                Address = address,
                Status = StatusPart_Time_Company.Inactive
            };

            _context.Set<Part_Time_Company>().Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> SetActiveAsync(Guid companyId)
        {
            var company = await _context.Set<Part_Time_Company>()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null) return false;

            company.Status = StatusPart_Time_Company.Active;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}