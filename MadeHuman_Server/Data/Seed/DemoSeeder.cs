using MadeHuman_Server.Model.Inbound;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Data.Seed
{
    public static class DemoSeeder
    {
        public static async Task SeedOutboundDemoDataAsync(ApplicationDbContext context)
        {
            Console.WriteLine("🔁 Bắt đầu seed dữ liệu kho Outbound...");

            var outboundZone = await context.WarehouseZones.FirstOrDefaultAsync(z => z.Name == "Outbound");
            if (outboundZone == null)
            {
                Console.WriteLine("⚠️ Không tìm thấy zone 'Outbound'");
                return;
            }
            Console.WriteLine($"✅ Tìm thấy zone 'Outbound': {outboundZone.Id}");

            var outboundLocations = await context.WarehouseLocations
                .Include(l => l.Inventory)
                .Where(l => l.ZoneId == outboundZone.Id)
                .ToListAsync();

            if (!outboundLocations.Any())
            {
                Console.WriteLine("⚠️ Không có WarehouseLocation nào trong zone 'Outbound'");
                return;
            }
            Console.WriteLine($"✅ Có {outboundLocations.Count} location trong Outbound");

            var skuIds = await context.OrderItems
                .Select(oi => oi.ProductSKUsId)
                .Distinct()
                .ToListAsync();

            Console.WriteLine($"✅ Có {skuIds.Count} ProductSKU từ OrderItems");

            var inventoryLogs = new List<InventoryLogs>();
            int index = 0;

            foreach (var skuId in skuIds)
            {
                var location = outboundLocations[index % outboundLocations.Count];
                var quantity = new Random().Next(50, 101);

                Console.WriteLine($"➡️ SKU {skuId} vào Location {location.Id} với SL = {quantity}");

                var inventory = location.Inventory;

                if (inventory != null)
                {
                    inventory.ProductSKUId = skuId;
                    inventory.StockQuantity = quantity;
                    inventory.QuantityBooked = 0;
                    inventory.LastUpdated = DateTime.UtcNow;

                    inventoryLogs.Add(new InventoryLogs
                    {
                        Id = Guid.NewGuid(),
                        InventoryId = inventory.Id, // ✅ Fix lỗi FK tại đây
                        ChangeQuantity = quantity,
                        StockQuantity = quantity,
                        RemainingQuantity = quantity,
                        ActionInventoryLogs = ActionInventoryLogs.Refill,
                        Time = DateTime.UtcNow,
                    });
                }
                else
                {
                    Console.WriteLine($"⚠️ Location {location.Id} chưa có Inventory → Bỏ qua!");
                }

                index++;
            }

            context.InventoryLogs.AddRange(inventoryLogs);
            Console.WriteLine($"💾 Đang lưu {inventoryLogs.Count} dòng log vào InventoryLogs...");
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Hoàn tất seed dữ liệu kho Outbound.");
        }



    }
}
