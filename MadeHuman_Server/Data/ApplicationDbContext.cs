using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Model.WareHouse;
using Madehuman_Share.ViewModel.WareHouse;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        // Thêm các DbSet cho từng model
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductSKU> ProductSKUs { get; set; }

        public DbSet<InboundReceipts> InboundReceipt { get; set; }
        public DbSet<InboundReceiptItems> InboundReceiptItems { get; set; }
        public DbSet<InboundTasks> InboundTasks { get; set; }
        public DbSet<ProductBatches> ProductBatches { get; set; }
        public DbSet<WareHouse> WareHouses { get; set; }
        public DbSet<WarehouseZones> WarehouseZones { get; set; }
        public DbSet<WarehouseLocations> WarehouseLocations { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryLogs> InventoryLogs { get; set; }









        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Quan trọng: gọi base trước

        

            // Đảm bảo mỗi ProductSKU phải thuộc về ProductItem HOẶC Combo
            modelBuilder.Entity<ProductSKU>()
                .HasCheckConstraint("CK_ProductSKU_Owner",
                    "([ProductItemId] IS NOT NULL AND [ComboId] IS NULL) OR ([ProductItemId] IS NULL AND [ComboId] IS NOT NULL)");
        }

        internal async Task<WareHouseViewModel> FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}
