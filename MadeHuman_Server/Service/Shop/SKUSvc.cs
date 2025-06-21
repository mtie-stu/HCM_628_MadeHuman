using MadeHuman_Server.Data;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Service.Shop
{
    public interface ISKUServices
    {
        Task<SKUInfoViewModel?> GetSKUDetailsAsync(string sku);
        Task<Guid?> GetProductSkuIdBySkuAsync(string sku);
    }
    public class SKUSvc  : ISKUServices
    {
        private readonly ApplicationDbContext _context;

        public SKUSvc(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SKUInfoViewModel?> GetSKUDetailsAsync(string sku)
        {
            var skuEntity = await _context.ProductSKUs
                .Include(s => s.Product)
                .Include(s => s.Combo)
                    .ThenInclude(c => c.ComboItems)
                        .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(s => s.SKU == sku);

            if (skuEntity == null) return null;

            var result = new SKUInfoViewModel
            {
                SKU = skuEntity.SKU
            };

            if (skuEntity.ProductId != null && skuEntity.Product != null)
            {
                result.Type = "Product";
                result.ProductId = skuEntity.Product.ProductId;
                result.ProductName = skuEntity.Product.Name;
                result.ProductPrice = skuEntity.Product.Price;
            }
            else if (skuEntity.ComboId != null && skuEntity.Combo != null)
            {
                result.Type = "Combo";
                result.ComboId = skuEntity.Combo.ComboId;
                result.ComboName = skuEntity.Combo.Name;
                result.ComboPrice = skuEntity.Combo.Price;

                result.ComboItems = skuEntity.Combo.ComboItems?
                    .Select(ci => new ComboItemViewModel
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product?.Name ?? "",
                        Quantity = ci.Quantity
                    }).ToList();
            }
            else
            {
                result.Type = "Unknown";
            }

            return result;
        }
        public async Task<Guid?> GetProductSkuIdBySkuAsync(string sku)
        {
            var productSku = await _context.ProductSKUs
                .Where(x => x.SKU == sku)
                .FirstOrDefaultAsync();

            return productSku?.Id;
        }

    }
}
