using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Model.User_Task;
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
        public DbSet<CheckInCheckOutLog> CheckInCheckOutLog { get; set; }
        public DbSet<UsersTasks> UsersTasks { get; set; }
        public DbSet<PartTimeAssignment> PartTimeAssignment { get; set; }







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

            // Tự động chuyển string => text (PostgreSQL)
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(string) && property.GetColumnType() == "nvarchar(max)")
                    {
                        property.SetColumnType("text"); // ✅ tương thích PostgreSQL
                    }
                }
            }

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

            modelBuilder.Entity<ProductSKU>()
      .HasCheckConstraint("CK_ProductSKU_Owner",
          "(\"ProductId\" IS NOT NULL AND \"ComboId\" IS NULL) OR (\"ProductId\" IS NULL AND \"ComboId\" IS NOT NULL)");

            modelBuilder.Entity<PartTimeAssignment>()
    .HasOne(p => p.PartTime)
    .WithMany()
    .HasForeignKey(p => p.PartTimeId)
    .OnDelete(DeleteBehavior.SetNull);

            /* // Đảm bảo mỗi ProductSKU phải thuộc về ProductItem HOẶC Combo
             modelBuilder.Entity<ProductSKU>()
                 .HasCheckConstraint("CK_ProductSKU_Owner",
                     "([ProductId] IS NOT NULL AND [ComboId] IS NULL) OR ([ProductId] IS NULL AND [ComboId] IS NOT NULL)");*/
        }

        internal async Task<WareHouseViewModel> FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
        public static async Task SeedPartTimeAssignmentAsync(ApplicationDbContext context)
        {
            if (await context.PartTimeAssignment.AnyAsync()) return;

            var today = DateTime.UtcNow.Date;

            var data = new List<PartTimeAssignment>
    {
        new PartTimeAssignment
        {
            Id = Guid.NewGuid(),
            PartTimeId = Guid.NewGuid(),
            WorkDate = today,
            TaskType = TaskType.Picker,
            ShiftCode = "Sáng",
            IsConfirmed = true,
            Note = "Có mặt đúng giờ"
        },
        new PartTimeAssignment
        {
            Id = Guid.NewGuid(),
            PartTimeId = Guid.NewGuid(),
            WorkDate = today,
            TaskType = TaskType.Packer,
            ShiftCode = "Chiều",
            IsConfirmed = false,
            Note = "Chưa xác nhận"
        },
        new PartTimeAssignment
        {
            Id = Guid.NewGuid(),
            PartTimeId = Guid.NewGuid(),
            WorkDate = today.AddDays(1),
            TaskType = TaskType.Dispatcher,
            ShiftCode = "Tối",
            IsConfirmed = true,
            Note = "Ca tăng cường"
        }
    };

            context.PartTimeAssignment.AddRange(data);
            await context.SaveChangesAsync();
        }

    }
}
