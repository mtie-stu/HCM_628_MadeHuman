using Madehuman_Share.ViewModel.Dashboard;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboard;
    public DashboardController(DashboardService dashboard) => _dashboard = dashboard;

    /// <summary>
    /// Donut OutboundTaskItems theo Status.
    /// GET /api/dashboard/outbound-status-mix?outboundTaskId={optional}
    /// </summary>
    [HttpGet("outbound-status-mix")]
    public async Task<ActionResult<List<DonutSliceOuntboundTaskItem>>> GetOutboundStatusMix(CancellationToken ct)
    {
        var data = await _dashboard.GetOutboundItemsStatusMixAsync(ct);
        return Ok(data);
    }

}
