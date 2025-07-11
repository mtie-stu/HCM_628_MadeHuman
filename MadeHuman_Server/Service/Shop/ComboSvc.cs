using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MadeHuman_Server.Service.Shop
{
    public interface IComboService
    {
        Task<IEnumerable<Combo>> GetAllAsync();
        Task<Combo?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(Guid id, CreateComboWithItemsViewModel combo);
        Task<Combo> CreateComboAsync(CreateComboViewModel vm);
        Task AddComboItemsAsync(AddComboItemsRequest request);
    }
    public class ComboService : IComboService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISkuGeneratorService _skuGenerator;
        private readonly GoogleDriveOAuthService _googleDriveService;

        public ComboService(ApplicationDbContext context, ISkuGeneratorService skuGenerator, GoogleDriveOAuthService googleDriveService)
        {
            _context = context;
            _skuGenerator = skuGenerator;
            _googleDriveService = googleDriveService;
        }

        public async Task<IEnumerable<Combo>> GetAllAsync()
        {
            return await _context.Combos
                .Include(c => c.ComboItems)
                //.Include(c => c.ComboItems).ThenInclude(ci => ci.Product)
                .Include(c => c.ProductSKU)
                .ToListAsync();
        }

            public async Task<Combo?> GetByIdAsync(Guid id)
            {
                return await _context.Combos
                    .Include(c => c.ComboItems)
                    .Include(c => c.ProductSKU)
                    .FirstOrDefaultAsync(c => c.ComboId == id);
            }



        public async Task<bool> UpdateAsync(Guid id, CreateComboWithItemsViewModel combo)
        {
            if (id != combo.ComboId)
                return false;

            var existing = await _context.Combos.FindAsync(id);
            if (existing == null)
                return false;

            existing.Name = combo.Name;
            existing.Description = combo.Description;
            existing.Price = combo.Price;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Combo> CreateComboAsync(CreateComboViewModel vm)
        {
            var comboId = Guid.NewGuid();
            var skuId = Guid.NewGuid();
            var sku = await _skuGenerator.GenerateUniqueSkuAsync();

            var combo = new Combo
            {
                ComboId = comboId,
                Name = vm.Name,
                Description = vm.Description,
                Price = 0, // sẽ tính sau

                ProductSKU = new ProductSKU
                {
                    Id = skuId,
                    SKU = sku,
                    ComboId = comboId
                }
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Combos.Add(combo);

                // ✅ Xử lý upload ảnh lên Google Drive
                if (vm.ImageFiles != null && vm.ImageFiles.Count > 0)
                {
                    foreach (var imageFile in vm.ImageFiles)
                    {
                        // 👉 Upload lên Google Drive và nhận URL
                        string imageUrl = await _googleDriveService.UploadFileAsync(imageFile);

                        _context.product_Combo_Imgs.Add(new Product_Combo_Img
                        {
                            ComboId = comboId,
                            ImageUrl = imageUrl
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return combo;
        }


        public async Task AddComboItemsAsync(AddComboItemsRequest request)
        {
            var combo = await _context.Combos.FirstOrDefaultAsync(c => c.ComboId == request.ComboId)
                        ?? throw new ArgumentException("Combo không tồn tại.");

            var comboItems = new List<ComboItem>();
            decimal totalPrice = 0m;

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Sản phẩm với ID {item.ProductId} không tồn tại.");

                comboItems.Add(new ComboItem
                {
                    ComboItemId = Guid.NewGuid(),
                    ComboId = combo.ComboId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });

                totalPrice += product.Price * item.Quantity;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.ComboItems.AddRange(comboItems);

                // Cập nhật tổng giá
                combo.Price = totalPrice;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
