using MadeHuman_Server.Data;
using MadeHuman_Server.Model.User_Task;
using System;

namespace MadeHuman_Server.Service.UserTask
{
    public interface IPartTimeService
    {
        Task<PartTime> CreateAsync(string partTimeId, string name, string cccd, string phone, Guid companyId);
    }
    public class PartTimeService : IPartTimeService
    {
        private readonly ApplicationDbContext _context;

        public PartTimeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PartTime> CreateAsync(string partTimeId, string name, string cccd, string phone, Guid companyId)
        {
            // Kiểm tra công ty có tồn tại không
            var company = await _context.Set<Part_Time_Company>().FindAsync(companyId);
            if (company == null)
                throw new Exception("Company không tồn tại.");

            var partTime = new PartTime
            {
                Id = Guid.NewGuid(),
                Name = name,
                CCCD = cccd,
                PhoneNumber = phone,
                CompanyId = companyId,
                StatusPartTimes=StatusPartTime.PartTime
               
            };

            _context.Set<PartTime>().Add(partTime);
            await _context.SaveChangesAsync();

            return partTime;
        }
    }
}
