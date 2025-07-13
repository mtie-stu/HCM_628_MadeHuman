using MadeHuman_Server.Data;
using MadeHuman_Server.Service.Shop;

using MadeHuman_Server.Model.Shop;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.EntityFrameworkCore;
using static MadeHuman_Server.Data.ApplicationDbContext;

namespace MadeHuman_Server.Service.Shop
{
    public interface IShopOrderService
    {
        Task<IEnumerable<ShopOrder>> GetAllAsync();
        Task<ShopOrder?> GetByIdAsync(Guid id);
        Task<ShopOrder> CreateAsync(CreateShopOrderWithMultipleItems vm);
        Task<bool> UpdateAsync(Guid id, ShopOrder updated);
        Task<List<ShopOrder>> CreateRandomOrdersWithSingleProductAsync(GenerateRandomOrdersSingleRequest request, string userId);
        Task<List<ShopOrder>> CreateRandomOrdersWithMultiSKUAsync(GenerateRandomMultiSKUOrdersRequest request);
    }
    public class ShopOrderService : IShopOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISKUServices _productSKUService;

        public ShopOrderService(ApplicationDbContext context, ISKUServices skuGeneratorService )
        {
            _context = context;
            _productSKUService= skuGeneratorService;    
        }

        public async Task<IEnumerable<ShopOrder>> GetAllAsync()
        {
            return await _context.ShopOrders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)                                                                        
                .ToListAsync();
        }

        public async Task<ShopOrder?> GetByIdAsync(Guid id)
        {
            return await _context.ShopOrders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductSKUs) // nếu muốn lấy SKUCode
                .FirstOrDefaultAsync(o => o.ShopOrderId == id);
        }

        public async Task<ShopOrder> CreateAsync(CreateShopOrderWithMultipleItems vm)
        {
            var orderId = Guid.NewGuid();

            var orderItems = vm.Items.Select(item => new OrderItem
            {
                OrderItemId = Guid.NewGuid(),
                ProductSKUsId = item.ProductSKUsId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                ShopOrderId = orderId
            }).ToList();


            var totalAmount = orderItems.Sum(x => x.Quantity * x.UnitPrice);
            // 🔥 Sửa ở đây: đảm bảo OrderDate có Kind = Utc
            var orderDate = vm.OrderDate == default
                ? DateTime.UtcNow
                : DateTime.SpecifyKind(vm.OrderDate, DateTimeKind.Utc); // ✅ FIX

            var order = new ShopOrder
            {
                ShopOrderId = orderId,
                AppUserId = vm.AppUserId,
                Status = (Model.Shop.StatusOrder)vm.Status,
                OrderDate = orderDate,
                TotalAmount = totalAmount,
                OrderItems = orderItems
            };

            _context.ShopOrders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }
    

        public async Task<bool> UpdateAsync(Guid id, ShopOrder updated)
        {
            if (id != updated.ShopOrderId)
                return false;

            var existing = await _context.ShopOrders.FindAsync(id);
            if (existing == null)
                return false;

            existing.TotalAmount = updated.TotalAmount;
            existing.Status = updated.Status;
            existing.AppUserId = updated.AppUserId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _context.ShopOrders.FindAsync(id);
            if (order == null)
                return false;

            _context.ShopOrders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }




        public async Task<List<ShopOrder>> CreateRandomOrdersWithSingleProductAsync(GenerateRandomOrdersSingleRequest request, string userId)
        {
            if (request.NumberOfOrders <= 0)
                throw new ArgumentException("Số lượng đơn hàng phải lớn hơn 0.");
            if (request.MinQuantity <= 0 || request.MaxQuantity < request.MinQuantity)
                throw new ArgumentException("Khoảng số lượng hàng hóa không hợp lệ.");

            var productExists = await _context.ProductSKUs.AnyAsync(p => p.Id == request.ProductSKUsId);
            if (!productExists)
                throw new ArgumentException("ProductSKU không tồn tại.");

            var random = new Random();
            var productskuid= await _productSKUService.GetProductSkuIdBySkuAsync(request.ProductSKUsId);
            var orders = new List<ShopOrder>();
            var appuserId = userId;

            for (int i = 0; i < request.NumberOfOrders; i++)
            {
                var quantity = random.Next(request.MinQuantity, request.MaxQuantity + 1);
                var orderId = Guid.NewGuid();

                var skuDetail = await _productSKUService.GetSKUDetailsAsync(request.ProductSKUsId);
                if (skuDetail == null)
                    throw new ArgumentException("SKU không hợp lệ.");

                decimal unitPrice;
                if (skuDetail.Type == "Product" && skuDetail.ProductPrice.HasValue)
                {
                    unitPrice = skuDetail.ProductPrice.Value;
                }
                else if (skuDetail.Type == "Combo" && skuDetail.ComboPrice.HasValue)
                {
                    unitPrice = skuDetail.ComboPrice.Value;
                }
                else
                {
                    throw new ArgumentException("Không thể xác định giá từ SKU.");
                }

                var orderItem = new OrderItem
                {
                    OrderItemId = Guid.NewGuid(),
                    ProductSKUsId = request.ProductSKUsId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    ShopOrderId = orderId,
                    //ProductSKUsId = productskuid.Value
                };

                var order = new ShopOrder
                {
                    ShopOrderId = orderId,
                    AppUserId = appuserId,
                    Status = (Model.Shop.StatusOrder)request.Status,
                    OrderDate = request.BaseOrderDate ?? DateTime.UtcNow,
                    TotalAmount = quantity * unitPrice,
                    OrderItems = new List<OrderItem> { orderItem }
                };

                orders.Add(order);
            }

            _context.ShopOrders.AddRange(orders);
            await _context.SaveChangesAsync();

            return orders;
        }
        public async Task<List<ShopOrder>> CreateRandomOrdersWithMultiSKUAsync(GenerateRandomMultiSKUOrdersRequest request)
        {
            if (request.TotalQuantity <= 0)
                throw new ArgumentException("Tổng số lượng đơn hàng phải lớn hơn 0.");

            var orders = new List<ShopOrder>();
            var random = new Random();

            // 1. Lấy danh sách SKU có liên kết Product hoặc Combo
            IQueryable<ProductSKU> skuQuery = _context.ProductSKUs
                .Include(sku => sku.Product)
                .Include(sku => sku.Combo);

            if (!request.IgnoreStock)
            {
                skuQuery = skuQuery.Where(sku => sku.QuantityInStock > 0);
            }

            if (request.CategoryId.HasValue)
            {
                skuQuery = skuQuery.Where(sku =>
                    sku.Product != null && sku.Product.CategoryId == request.CategoryId.Value);
            }

            var availableSKUs = await skuQuery.ToListAsync();
            if (!availableSKUs.Any())
                throw new InvalidOperationException("Không có SKU nào khả dụng để tạo đơn hàng.");

            // 2. Tạo đơn hàng
            for (int i = 0; i < request.TotalQuantity; i++)
            {
                var orderId = Guid.NewGuid();
                var orderItems = new List<OrderItem>();
                decimal totalAmount = 0;

                int numberOfSKUs = random.Next(1, Math.Min(5, availableSKUs.Count + 1));
                var selectedSKUs = availableSKUs
                    .OrderBy(x => Guid.NewGuid())
                    .Take(numberOfSKUs)
                    .ToList();

                foreach (var sku in selectedSKUs)
                {
                    // Xác định đơn giá
                    decimal? unitPrice = sku.Product?.Price ?? sku.Combo?.Price;
                    if (unitPrice == null || unitPrice <= 0) continue;

                    // Xác định số lượng
                    int qty;
                    if (request.IgnoreStock)
                    {
                        qty = random.Next(1, 6);
                    }
                    else
                    {
                        if (sku.QuantityInStock <= 0) continue;
                        var maxQty = Math.Min(sku.QuantityInStock, 10);
                        qty = random.Next(1, maxQty + 1);
                        sku.QuantityInStock -= qty;
                    }

                    var item = new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        ProductSKUsId = sku.Id,
                        Quantity = qty,
                        UnitPrice = unitPrice.Value,
                        ShopOrderId = orderId
                    };

                    orderItems.Add(item);
                    totalAmount += qty * unitPrice.Value;
                }

                if (!orderItems.Any()) continue;

                var order = new ShopOrder
                {
                    ShopOrderId = orderId,
                    AppUserId = request.AppUserId,
                    OrderDate = DateTime.UtcNow,
                    Status = Model.Shop.StatusOrder.Created,
                    TotalAmount = totalAmount,
                    OrderItems = orderItems
                };

                orders.Add(order);
            }

            _context.ShopOrders.AddRange(orders);
            await _context.SaveChangesAsync();

            return orders;
        }



    }
}
