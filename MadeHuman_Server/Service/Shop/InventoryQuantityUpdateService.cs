using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MadeHuman_Server.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class InventoryQuantityUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InventoryQuantityUpdateService> _logger;

    public InventoryQuantityUpdateService(IServiceScopeFactory scopeFactory, ILogger<InventoryQuantityUpdateService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var skuList = await db.ProductSKUs
                    .Include(s => s.Inventory)
                    .ToListAsync(stoppingToken);

                foreach (var sku in skuList)
                {
                    var totalStock = sku.Inventory.Sum(i => i.StockQuantity);
                    sku.QuantityInStock = totalStock;
                }

                await db.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("✅ Cập nhật số lượng tồn kho SKU thành công lúc {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi cập nhật số lượng tồn kho SKU");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
