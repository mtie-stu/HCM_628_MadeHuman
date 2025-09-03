using MadeHuman_Server.Data; // DbContext của bạn
using MadeHuman_Server.Model.Outbound;
using Madehuman_Share.ViewModel.Dashboard;
using Microsoft.EntityFrameworkCore;
using System;

public class DashboardService
{
    private readonly ApplicationDbContext _context;
    public DashboardService(ApplicationDbContext context) => _context = context;

    /// <summary>
    /// Đếm OutboundTaskItems theo Status để vẽ donut.
    /// Có thể lọc theo OutboundTaskId (tùy chọn).
    /// </summary>
    public async Task<List<DonutSliceOuntboundTaskItem>> GetOutboundItemsStatusMixAsync(
    CancellationToken ct = default)
    {
        // Đếm số lượng theo Status trên toàn bộ OutboundTaskItems
        var counts = await _context.OutboundTaskItems
            .AsNoTracking()
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        // Bảo đảm trả đủ mọi trạng thái (kể cả = 0)
        var all = Enum.GetValues<StatusOutboundTaskItems>()
            .Cast<StatusOutboundTaskItems>()
            .Select(s => new DonutSliceOuntboundTaskItem
            {
                Label = s.ToString(),
                Count = counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0
            })
            .ToList();

        return all;
    }

}
