using MadeHuman_Server.Service.Outbound;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Outbound
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // đảm bảo có user khi truy cập
    public class CheckTaskController : ControllerBase
    {
        private readonly ICheckTaskServices _checkTaskService;

        public CheckTaskController(ICheckTaskServices checkTaskService)
        {
            _checkTaskService = checkTaskService;
        }

        /// <summary>
        /// Tạo nhiệm vụ kiểm hàng từ OutboundTaskId
        /// </summary>
        //[HttpPost("create/{outboundTaskId}")]
        //public async Task<IActionResult> CreateCheckTask(Guid outboundTaskId)
        //{
        //    var result = await _checkTaskService.CreateCheckTaskAsync(outboundTaskId);
        //    return Ok(result);
        //}

        /// <summary>
        /// Gán người dùng hiện tại vào nhiệm vụ kiểm hàng thông qua basketId
        /// </summary>
        [HttpPost("assign-user/{basketId}")]
        public async Task<IActionResult> AssignUserToCheckTask(Guid basketId)
        {
            try
            {
                var result = await _checkTaskService.AssignUserTaskToCheckTaskByBasketAsync(basketId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Quét SKU để xác nhận kiểm
        /// </summary>
        [HttpPost("scan-mix-sku")]
        public async Task<IActionResult> ValidateScan([FromBody] ScanCheckTaskRequest request)
        {
            var result = await _checkTaskService.ValidateCheckTaskScanAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Gán slot theo thứ tự đơn hàng
        /// </summary>
        [HttpPost("assign-slot")]
        public async Task<IActionResult> AssignSlot([FromBody] AssignSlotRequest request)
        {
            var result = await _checkTaskService.AssignSlotAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Xác nhận kiểm hàng thủ công nếu chỉ có 1 SKU
        /// </summary>
        [HttpPost("single-sku")]
        public async Task<IActionResult> ValidateSingleSKU([FromBody] SingleSKUCheckTaskRequest request)
        {
            var result = await _checkTaskService.ValidateSingleSKUCheckTaskAsync(request);
            return Ok(result);
        }

        [HttpGet("preview-single-sku/{basketId}/{sku}")]
        public async Task<IActionResult> PreviewSingleSKU(Guid basketId, string sku)
        {
            var result = await _checkTaskService.PreviewSingleSKUAsync(basketId, sku);
            if (result == null)
                return NotFound(new { message = "❌ SKU không hợp lệ trong nhiệm vụ này." });

            return Ok(result);
        }
        [HttpPost("mix-sku/validate")]
        public async Task<IActionResult> ValidateMixSKU([FromBody] ValidateMixCheckTaskRequest request)
        {
            if (request == null || request.CheckTaskDetailId == Guid.Empty || string.IsNullOrWhiteSpace(request.SKU))
                return BadRequest(new { code = 0, logs = new[] { "❌ Dữ liệu không hợp lệ." } });

            try
            {
                var (code, logs) = await _checkTaskService.ValidateMixCheckTaskScanAsync(request);

                // code: 0 = lỗi, 1|2|3 = thành công với các nhánh FE đã quy ước
                if (code == 0)
                    return BadRequest(new { code, logs });

                return Ok(new { code, logs });
            }
            catch (Exception ex)
            {
                // Có thể log ex tại đây
                return StatusCode(500, new { code = 0, logs = new[] { "❌ Lỗi xử lý: " + ex.Message } });
            }


        }

        /// <summary>
        /// Lấy danh sách log theo CheckTaskId (log mới nhất ở đầu)
        /// </summary>
        /// <param name="checkTaskId">Id của CheckTask</param>
        [HttpGet("{checkTaskId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<CheckTaskLogViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CheckTaskLogViewModel>>> GetByCheckTaskId(
            Guid checkTaskId,
            CancellationToken cancellationToken)
        {
            // Service đã xử lý sắp xếp theo Time desc và map sang ViewModel
            var logs = await _checkTaskService.GetLogsByCheckTaskIdAsync(checkTaskId);
            return Ok(logs);
        }
    }
}
