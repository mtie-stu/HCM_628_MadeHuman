using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Data;

public class ReceiptStatusUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public ReceiptStatusUpdaterService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateCreatedReceiptsToConfirmedAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task UpdateCreatedReceiptsToConfirmedAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var receiptsToUpdate = await dbContext.InboundReceipt
                .Where(r => r.Status == StatusReceipt.Created)
                .ToListAsync();

            foreach (var receipt in receiptsToUpdate)
            {
                receipt.Status = StatusReceipt.Confirmed;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
