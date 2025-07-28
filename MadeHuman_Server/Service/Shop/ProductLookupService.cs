using MadeHuman_Server.Data;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Shop
{
    public interface IProductLookupService
    {
        Task<ProductSKUInfoViewmodel?> GetSKUInfoAsync(Guid productSKUId);
    }

    public class ProductLookupService : IProductLookupService
    {
        private readonly ApplicationDbContext _context;

        public ProductLookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductSKUInfoViewmodel?> GetSKUInfoAsync(Guid productSKUId)
        {
            var sku = await _context.ProductSKUs
                .Where(s => s.Id == productSKUId)
                .Select(s => new ProductSKUInfoViewmodel
                {
                    ProductSKUId = s.Id,
                    SkuCode = s.SKU,
                    ProductName = s.Product.Name,
                    ImageUrls = _context.product_Combo_Imgs
                        .Where(img => img.ProductId == s.ProductId)
                        .Select(img => img.ImageUrl)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return sku;
        }

    }
}
