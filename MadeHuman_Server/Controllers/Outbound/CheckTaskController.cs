using MadeHuman_Server.Service.Outbound;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Outbound
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // đảm bảo có user khi truy cập
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
    }
}
