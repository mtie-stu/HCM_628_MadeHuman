
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using MadeHuman_Server.Data;
using MadeHuman_Server.Model.User_Task;


namespace MadeHuman_Server.Service.UserTask
{
    public class ResetHourlyKPIsService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;


        public ResetHourlyKPIsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextHour = now.AddHours(1);
                var nextRoundHour = new DateTime(nextHour.Year, nextHour.Month, nextHour.Day, nextHour.Hour, 0, 0);
                var delay = nextRoundHour - now;

                await Task.Delay(delay, stoppingToken); // ⏳ Delay đến giờ tròn

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var tasks = await db.UsersTasks
                        .Where(x => !x.IsCompleted)
                        .ToListAsync(stoppingToken);

                    foreach (var t in tasks)
                    {
                        t.HourlyKPIs = 0;
                    }

                    await db.SaveChangesAsync(stoppingToken);
                    Console.WriteLine($"✅ Reset HourlyKPIs lúc {DateTime.Now:HH:mm:ss}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi khi reset HourlyKPIs: {ex.Message}");
                }
            }
        }

    }
}
