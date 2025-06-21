using MadeHuman_Server.Model;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Model.WareHouse;
using Madehuman_Share.ViewModel.WareHouse;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<Product_Combo_Img> product_Combo_Imgs { get; set; }







        public static class DefaultData
        {
            public static readonly string FakeUserId = "ab789a74-0e4e-4cd8-8918-b8da35610b14"; // cố định
        }

        public static async Task SeedFakeUserAsync(UserManager<AppUser> userManager)
        {
            var fakeUser = await userManager.FindByIdAsync(DefaultData.FakeUserId);
            if (fakeUser != null) return;

            var user = new AppUser
            {
                Id = DefaultData.FakeUserId, // dùng ID cố định
                UserName = "fake_user",
                Email = "fake_user@app.com",
                EmailConfirmed = true,
                Name = "Người dùng ảo",
                UserTypes = 0
            };

            await userManager.CreateAsync(user, "Fake123!");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Quan trọng: gọi base trước

            modelBuilder.Entity<Product>()
        .HasOne(p => p.ProductSKU)
        .WithOne(sku => sku.Product)
        .HasForeignKey<ProductSKU>(sku => sku.ProductId)
        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Combo>()
          .HasOne(c => c.ProductSKU)
          .WithOne(sku => sku.Combo)
          .HasForeignKey<ProductSKU>(sku => sku.ComboId)
          .OnDelete(DeleteBehavior.Restrict);


            // Đảm bảo mỗi ProductSKU phải thuộc về ProductItem HOẶC Combo
            modelBuilder.Entity<ProductSKU>()
                .HasCheckConstraint("CK_ProductSKU_Owner",
                    "([ProductId] IS NOT NULL AND [ComboId] IS NULL) OR ([ProductId] IS NULL AND [ComboId] IS NOT NULL)");
        }

        internal async Task<WareHouseViewModel> FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}
