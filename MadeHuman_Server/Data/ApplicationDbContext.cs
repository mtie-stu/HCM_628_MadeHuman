using MadeHuman_Server.Model.Shop;
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



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Quan trọng: gọi base trước

        

            // Đảm bảo mỗi ProductSKU phải thuộc về ProductItem HOẶC Combo
            modelBuilder.Entity<ProductSKU>()
                .HasCheckConstraint("CK_ProductSKU_Owner",
                    "([ProductItemId] IS NOT NULL AND [ComboId] IS NULL) OR ([ProductItemId] IS NULL AND [ComboId] IS NOT NULL)");
        }
    }
}
