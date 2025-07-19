using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Outbound;
using Madehuman_Share.ViewModel.Basket;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IBasketService
    {
        Task<List<BasketViewModel>> AddRangeAsync(CreateBasketRangeViewModel model);

        Task<BasketViewModel> AddAsync(CreateBasketViewModel model);
        Task<BasketViewModel> GetByIdAsync(Guid id);
        Task<List<BasketViewModel>> GetAllAsync();
        Task<bool> UpdateOutBoundTaskIdAsync(UpdateBasketOutboundTaskViewModel model);
    }

    public class BasketService : IBasketService
    {
        private readonly ApplicationDbContext _context;

        public BasketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BasketViewModel> AddAsync(CreateBasketViewModel model)
        {
            var basket = new Baskets
            {
                Id = Guid.NewGuid(),
                Status = StatusBaskets.Empty,
                OutBoundTaskId = model.OutBoundTaskId
            };

            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(basket.Id);
        }

        public async Task<BasketViewModel> GetByIdAsync(Guid id)
        {
            var basket = await _context.Baskets
                .Include(b => b.OutboundTask)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (basket == null) return null;

            return new BasketViewModel
            {
                Id = basket.Id,
                Status = basket.Status.ToString(),
              
            };
        }

        public async Task<List<BasketViewModel>> GetAllAsync()
        {
            return await _context.Baskets
                .Include(b => b.OutboundTask)
                .Select(b => new BasketViewModel
                {
                    Id = b.Id,
                    Status = b.Status.ToString(),
                    OutBoundTaskId = b.OutBoundTaskId,
               
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateOutBoundTaskIdAsync(UpdateBasketOutboundTaskViewModel model)
        {
            var basket = await _context.Baskets.FindAsync(model.BasketId);
            if (basket == null) return false;

            // Gán OutBoundTaskId
            basket.OutBoundTaskId = model.OutBoundTaskId;

            // Nếu là null hoặc Guid.Empty thì xóa liên kết + set trạng thái = Empty
            if (model.OutBoundTaskId == null || model.OutBoundTaskId == Guid.Empty)
            {
                basket.OutBoundTaskId = null;
                basket.Status = StatusBaskets.Empty;
            }
            else
            {
                // Nếu có Status truyền từ client thì dùng, còn không thì mặc định là Selected
                basket.Status = model.Status.HasValue
                    ? (StatusBaskets)model.Status.Value
                    : StatusBaskets.Selected;
            }

            _context.Baskets.Update(basket);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<BasketViewModel>> AddRangeAsync(CreateBasketRangeViewModel model)
        {
            var result = new List<BasketViewModel>();
            var basketList = new List<Baskets>();

            for (int i = 0; i < model.Quantity; i++)
            {
                var basket = new Baskets
                {
                    Id = Guid.NewGuid(),
                    Status = StatusBaskets.Empty,
                    OutBoundTaskId = null
                };

                basketList.Add(basket);
            }

            _context.Baskets.AddRange(basketList);
            await _context.SaveChangesAsync();

            // Lấy lại để trả về dạng view model
            foreach (var b in basketList)
            {
                result.Add(new BasketViewModel
                {
                    Id = b.Id,
                    Status = b.Status.ToString(),
                    OutBoundTaskId = null,
                });
            }

            return result;
        }


    }
}
