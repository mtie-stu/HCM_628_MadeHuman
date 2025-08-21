using MadeHuman_Server.Service.Outbound;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MadeHuman_Server.Controllers.Outbound
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OutboundTaskController : ControllerBase
    {
        private readonly IOutboundTaskServices _outboundTaskService;

        public OutboundTaskController(IOutboundTaskServices outboundTaskService)
        {
            _outboundTaskService = outboundTaskService;
        }

        /// <summary>
        /// Tạo OutboundTask cho các đơn hàng có 1 SKU thường (Product)
        /// </summary>
        [HttpPost("single-product")]
        public async Task<IActionResult> CreateSingleProductTasks()
        {
            var result = await _outboundTaskService.CreateOutboundTaskSingleProductAsync();
            return Ok(new
            {
                message = $"✅ Đã tạo {result.Count} OutboundTask (1 SKU - Product).",
                tasks = result
            });
        }

        /// <summary>
        /// Tạo OutboundTask cho các đơn hàng có 1 SKU dạng Combo
        /// </summary>
        [HttpPost("single-mix-product")]
        public async Task<IActionResult> CreateSingleMixProductTasks()
        {
            var result = await _outboundTaskService.CreateOutboundTaskSingleMixProductAsync();
            return Ok(new
            {
                message = $"✅ Đã tạo {result.Count} OutboundTask (1 SKU - Combo).",
                tasks = result
            });
        }

        /// <summary>
        /// Tạo OutboundTask cho các đơn hàng có nhiều SKU (multi)
        /// </summary>
        [HttpPost("multi-product")]
        public async Task<IActionResult> CreateMultiProductTasks()
        {
            var result = await _outboundTaskService.CreateOutboundTaskMultiProductAsync();
            return Ok(new
            {
                message = $"✅ Đã tạo {result.Count} OutboundTask (multi SKU).",
                tasks = result
            });
        }

        /// <summary>
        /// Chạy toàn bộ quy trình xử lý đơn hàng: single, mix, multi cho đến khi không còn đơn
        /// </summary>
        [HttpPost("process-all")]
        public async Task<IActionResult> RunAllProcessing()
        {
            var total = await (_outboundTaskService as OutboundTaskService)?.RunAllOutboundTaskProcessingAsync()!;
            return Ok(new
            {
                message = $"✅ Đã xử lý tổng cộng {total} OutboundTask.",
                totalCreated = total
            });
        }
        [HttpGet("{id:guid}/preview")]
        // [Authorize] // bật nếu cần JWT
        public async Task<ActionResult<OutboundTaskItemPreviewViewModel>> Preview(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest(new { message = "Id không hợp lệ." });

            var dto = await _outboundTaskService.GetPreviewAsync(id, ct);
            if (dto == null) return NotFound(new { message = "Không tìm thấy OutboundTaskItem." });

            return Ok(dto);
        }
    }
}
