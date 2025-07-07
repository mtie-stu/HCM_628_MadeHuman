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
            : base(options) { }

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
        public DbSet<PartTime> PartTimes { get; set; }
        public DbSet<Part_Time_Company> PartTimeCompanies { get; set; }

        public static class DefaultData
        {
            public static readonly string FakeUserId = "ab789a74-0e4e-4cd8-8918-b8da35610b14";
        }

        public static async Task SeedFakeUserAsync(UserManager<AppUser> userManager)
        {
            var fakeUser = await userManager.FindByIdAsync(DefaultData.FakeUserId);
            if (fakeUser != null) return;

            var user = new AppUser
            {
                Id = DefaultData.FakeUserId,
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
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(string) && property.GetColumnType() == "nvarchar(max)")
                    {
                        property.SetColumnType("text");
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
        }

        public static async Task SeedPartTimeAsync(ApplicationDbContext context)
        {
            if (await context.PartTimes.AnyAsync()) return;

            var company = await context.PartTimeCompanies.FirstOrDefaultAsync();
            if (company == null)
            {
                company = new Part_Time_Company
                {
                    Id = Guid.NewGuid(),
                    Name = "Công ty ABC",
                    Status = StatusPart_Time_Company.Active
                };
                context.PartTimeCompanies.Add(company);
                await context.SaveChangesAsync();
            }

            var partTimes = new List<PartTime>
            {
                new PartTime { PartTimeId = Guid.NewGuid(), Name = "Nguyễn Văn A", CCCD = "001", PhoneNumber = "0900000001", StatusPartTimes = StatusPartTime.PartTime, CompanyId = company.Id },
                new PartTime { PartTimeId = Guid.NewGuid(), Name = "Trần Thị B", CCCD = "002", PhoneNumber = "0900000002", StatusPartTimes = StatusPartTime.PTCD, CompanyId = company.Id },
                new PartTime { PartTimeId = Guid.NewGuid(), Name = "Lê Văn C", CCCD = "003", PhoneNumber = "0900000003", StatusPartTimes = StatusPartTime.Banned, CompanyId = company.Id }
            };

            context.PartTimes.AddRange(partTimes);
            await context.SaveChangesAsync();
        }

      /*  public static async Task SeedPartTimeAssignmentAsync(ApplicationDbContext context)
        {
            if (await context.PartTimeAssignment.AnyAsync()) return;

            var today = DateTime.UtcNow.Date;
            var partTimes = await context.PartTimes.Take(3).ToListAsync();
            if (partTimes.Count < 3) return;

            var data = new List<PartTimeAssignment>
            {
                new PartTimeAssignment
                {
                    Id = Guid.NewGuid(),
                    PartTimeId = partTimes[0].PartTimeId,
                    WorkDate = today,
                    TaskType = TaskType.Picker,
                    ShiftCode = "Sáng",
                    IsConfirmed = true,
                    Note = "Có mặt đúng giờ",
                    CompanyId = partTimes[0].CompanyId
                },
                new PartTimeAssignment
                {
                    Id = Guid.NewGuid(),
                    PartTimeId = partTimes[1].PartTimeId,
                    WorkDate = today,
                    TaskType = TaskType.Packer,
                    ShiftCode = "Chiều",
                    IsConfirmed = false,
                    Note = "Chưa xác nhận",
                    CompanyId = partTimes[1].CompanyId
                },
                new PartTimeAssignment
                {
                    Id = Guid.NewGuid(),
                    PartTimeId = partTimes[2].PartTimeId,
                    WorkDate = today.AddDays(1),
                    TaskType = TaskType.Dispatcher,
                    ShiftCode = "Tối",
                    IsConfirmed = true,
                    Note = "Ca tăng cường",
                    CompanyId = partTimes[2].CompanyId
                }
            };

            context.PartTimeAssignment.AddRange(data);
            await context.SaveChangesAsync();
        }*/
    }
}
