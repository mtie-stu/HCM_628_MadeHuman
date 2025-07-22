using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MadeHuman_Server.Service.Outbound
{
    public class OutboundTaskBackgroundService : BackgroundService
    {
        private readonly ILogger<OutboundTaskBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public OutboundTaskBackgroundService(ILogger<OutboundTaskBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🔁 OutboundTaskBackgroundService đang khởi động...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var outboundService = scope.ServiceProvider.GetRequiredService<OutboundTaskService>();
                        int count = await outboundService.RunAllOutboundTaskProcessingAsync();
                        _logger.LogInformation($"✅ Background xử lý xong {count} OutboundTask.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Lỗi khi chạy background OutboundTask.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // lặp mỗi 5 phút
            }
        }
    }
}
