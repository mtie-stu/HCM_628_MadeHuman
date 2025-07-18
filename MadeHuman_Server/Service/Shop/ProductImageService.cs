using MadeHuman_Server.Data;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Shop
{
    public interface IProductImageService
    {
        Task<Dictionary<Guid, List<string>>> GetImageUrlsByProductSKUIdsAsync(List<Guid> productSKUIds);
    }
    public class ProductImageService : IProductImageService
    {
        private readonly ApplicationDbContext _context;

        public ProductImageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<Guid, List<string>>> GetImageUrlsByProductSKUIdsAsync(List<Guid> productSKUIds)
        {
            var productSkus = await _context.ProductSKUs
                .Where(sku => productSKUIds.Contains(sku.Id))
                .ToListAsync();

            var result = new Dictionary<Guid, List<string>>();

            foreach (var sku in productSkus)
            {
                List<string> imageUrls;

                if (sku.ProductId != null)
                {
                    imageUrls = await _context.product_Combo_Imgs
                        .Where(img => img.ProductId == sku.ProductId)
                        .Select(img => img.ImageUrl)
                        .ToListAsync();
                }
                else if (sku.ComboId != null)
                {
                    imageUrls = await _context.product_Combo_Imgs
                        .Where(img => img.ComboId == sku.ComboId)
                        .Select(img => img.ImageUrl)
                        .ToListAsync();
                }
                else
                {
                    imageUrls = new List<string>();
                }

                result[sku.Id] = imageUrls;
            }

            return result;
        }

    }
}
