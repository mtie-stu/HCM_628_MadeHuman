using MadeHuman_Server.Service.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Outbound
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillRenderService _billRenderService;

        public BillController(IBillRenderService billRenderService)
        {
            _billRenderService = billRenderService;
        }

        /// <summary>
        /// Lấy danh sách HTML bill theo CheckTaskId
        /// </summary>
        [HttpGet("print-bills/{checkTaskId}")]
        public async Task<IActionResult> GetBills(Guid checkTaskId)
        {
            var htmlList = await _billRenderService.GenerateBillHtmlByCheckTaskIdAsync(checkTaskId);
            return Ok(htmlList);
        }

        [HttpGet("by-check-detail/{checkTaskDetailId}")]
        public async Task<IActionResult> GetByCheckDetailId(Guid checkTaskDetailId)
        {
            var result = await _billRenderService.GenerateBillHtmlByCheckTaskDetailIdAsync(checkTaskDetailId);
            return Ok(result);
        }

    }
}