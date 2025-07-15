using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Outbound;
using Madehuman_Share;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MadeHuman_Server.Services
{
    public class BasketService
    {
        private readonly ApplicationDbContext _context;

        public BasketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BasketViewModel>> GetAllAsync()
        {
            return await _context.Baskets
                .Select(b => new BasketViewModel
                {
                    Id = b.Id,
                    Status = (int)b.Status,
                    OutBoundTaskId = null
                }).ToListAsync();
        }

        public async Task<BasketViewModel> GetByIdAsync(Guid id)
        {
            var basket = await _context.Baskets.FindAsync(id);
            if (basket == null) return null;

            return new BasketViewModel
            {
                Id = basket.Id,
                Status = (int)basket.Status,
                OutBoundTaskId = null
            };
        }

        public async Task<Guid> CreateAsync(BasketViewModel model)
        {
            var basket = new Baskets
            {
                Id = Guid.NewGuid(),
                Status = (StatusBaskets)model.Status,
                OutBoundTaskId = null
            };

            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();
            return basket.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, BasketViewModel model)
        {
            var basket = await _context.Baskets.FindAsync(id);
            if (basket == null) return false;

            basket.Status = (StatusBaskets)model.Status;
            basket.OutBoundTaskId = model.OutBoundTaskId;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
