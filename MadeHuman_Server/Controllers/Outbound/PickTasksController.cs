using MadeHuman_Server.Service.Outbound;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Outbound
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Bắt buộc xác thực
    public class PickTasksController : ControllerBase
    {
        private readonly IPickTaskServices _pickTaskServices;

        public PickTasksController(IPickTaskServices pickTaskServices)
        {
            _pickTaskServices = pickTaskServices;
        }

        /// <summary>
        /// Gán PickTask đầu tiên chưa gán cho user hiện tại
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignPickTask()
        {
            var result = await _pickTaskServices.AssignPickTaskToCurrentUserAsync();
            if (result == null)
                return Ok(new { message = "🎉 Hiện tại không còn PickTask nào cần xử lý." });

            return Ok(result);
        }

        /// <summary>
        /// Quét PickTaskDetail (gồm: SKU, BasketId, PickTaskDetailId) để ghi nhận một lần pick
        /// </summary>
        [HttpPost("scan")]
        public async Task<IActionResult> ValidatePickTaskScan([FromBody] ScanPickTaskValidationRequest request)
        {
            var messages = await _pickTaskServices.ValidatePickTaskScanAsync(request);

            if (messages.Any(m => m.StartsWith("❌")))
                return BadRequest(new { errors = messages });

            return Ok(new { messages });
        }
    }
}
