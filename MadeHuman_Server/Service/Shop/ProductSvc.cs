using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Shop
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(CreateProduct_ProdcutSKU_ViewModel createproduct);
        Task<bool> UpdateAsync(Guid id, CreateProduct_ProdcutSKU_ViewModel updated);
        Task<bool> DeleteAsync(Guid id);
    }
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
         private readonly GoogleDriveService _googleDriveService;

        public ProductService(ApplicationDbContext context, GoogleDriveService googleDriveService)
        {
            _context = context;
            _googleDriveService = googleDriveService;

        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductItems)
                .Include(p => p.ProductSKU)
                .Include(p => p.ComboItems)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductItems)
                .Include(p => p.ProductSKU)
                .Include(p => p.ComboItems)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> CreateAsync(CreateProduct_ProdcutSKU_ViewModel createproduct)
        {
            var productId = Guid.NewGuid();

            var product = new Product
            {
                ProductId = productId,
                Name = createproduct.Name,
                Description = createproduct.Description,
                Price = createproduct.Price,
                CategoryId = createproduct.CategoryId,

                ProductSKU = new ProductSKU
                {
                    Id = Guid.NewGuid(),
                    SKU = createproduct.SKU,
                    ProductId = productId
                },

                ProductItems = new List<ProductItem>
        {
            new ProductItem
            {
                ProductItemId = Guid.NewGuid(),
                ProductId = productId,
                SKU = createproduct.SKU,
            }
        }
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // ✅ Xử lý ảnh: upload lên Google Drive
            if (createproduct.ImageFiles != null && createproduct.ImageFiles.Count > 0)
            {
                foreach (var imageFile in createproduct.ImageFiles)
                {
                    // 👉 Upload ảnh lên Google Drive
                    string imageUrl = await _googleDriveService.UploadFileAsync(imageFile);

                    var imageRecord = new Product_Combo_Img
                    {
                        ProductId = productId,
                        ImageUrl = imageUrl // ✅ Lưu URL ảnh Drive
                    };

                    _context.product_Combo_Imgs.Add(imageRecord);
                }

                await _context.SaveChangesAsync();
            }

            return product;
        }



        public async Task<bool> UpdateAsync(Guid id, CreateProduct_ProdcutSKU_ViewModel updated)
        {
            if (id != updated.ProductId)
                return false;

            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return false;

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Price = updated.Price;
            existing.CategoryId = updated.CategoryId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

      
    }
}
