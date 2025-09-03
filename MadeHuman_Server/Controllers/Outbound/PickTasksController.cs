using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Service.Outbound;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Controllers.Outbound
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Bắt buộc xác thực
    public class PickTasksController : ControllerBase
    {
        private readonly IPickTaskServices _pickTaskServices;
        private readonly ApplicationDbContext _Context;

        public PickTasksController(IPickTaskServices pickTaskServices, ApplicationDbContext dbContext)
        {
            _pickTaskServices = pickTaskServices;
            _Context = dbContext;
        }

        /// <summary>
        /// ✅ Gán PickTask đầu tiên chưa gán cho user hiện tại
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignPickTask()
        {
            var taskId = await _pickTaskServices.AssignPickTaskToCurrentUserAsync();

            if (taskId == null)
                return Ok(new { message = "🎉 Hiện tại không có PickTask nào cần xử lý." });

            return Ok(new { id = taskId });
        }


        /// <summary>
        /// ✅ Quét SKU, Vị trí và Giỏ (BasketId) để ghi nhận Pick 1 sản phẩm
        /// </summary>
        [HttpPost("scan")]
        public async Task<IActionResult> ValidatePickTaskScan([FromBody] ScanPickTaskValidationRequest request)
        {
            var messages = await _pickTaskServices.ValidatePickTaskScanAsync(request);

            if (messages.Any(m => m.StartsWith("❌")))
                return BadRequest(new { errors = messages });

            return Ok(new { messages });
        }

        /// <summary>
        /// ✅ Lấy danh sách PickTask đã gán cho người dùng hiện tại (dùng cho View Index)
        /// </summary>
        [HttpGet("mine")]
        public async Task<IActionResult> GetByUsersTaskId()
        {
            var result = await _pickTaskServices.GetPickTasksByUserTaskIdAsync();
            return Ok(result);
        }

        /// <summary>
        /// ✅ Xác nhận hoàn tất PickTaskDetail khi quét giỏ và đủ số lượng
        /// </summary>
        [HttpPost("confirm-basket")]
        public async Task<IActionResult> ConfirmPickDetailWithBasket([FromBody] ConfirmPickDetailRequest request)
        {
            var result = await _pickTaskServices.ConfirmPickDetailToBasketAsync(
                request.PickTaskId, request.PickTaskDetailId, request.BasketId);

            if (result.Messages.Any(m => m.StartsWith("❌")))
                return BadRequest(new { errors = result.Messages });

            return Ok(new
            {
                messages = result.Messages,
                isPickTaskFinished = result.IsPickTaskFinished,
                nextDetail = result.NextDetail
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPickTaskDetailById(Guid id)
        {
            var result = await _pickTaskServices.GetPickTaskDetailByIdAsync(id);


            return Ok(result);
        }
        /// <summary>
        /// Gán giỏ vào nhiệm vụ Outbound
        /// </summary>
        [HttpPost("assign-basket")]
        public async Task<IActionResult> AssignBasketToOutbound([FromBody] ConfirmBasketRequest request)
        {
            var outboundTaskId = await _Context.PickTasks
                .Where(b => b.Id == request.PickTaskId)
                .Select(b => b.OutboundTaskId)
                .FirstOrDefaultAsync();
            if (request == null || request.BasketId == Guid.Empty || outboundTaskId == Guid.Empty)
            {
                return BadRequest("Thiếu thông tin BasketId hoặc OutboundTaskId.");
            }

            var result = await _pickTaskServices.AssignBasketToOutboundTaskAsync(request.BasketId, outboundTaskId);
            if (result.Any(m => m.StartsWith("❌")))
                return BadRequest(new { Errors = result });

            return Ok(new { Message = "✅ Gán giỏ thành công." });
        }
    }
}

