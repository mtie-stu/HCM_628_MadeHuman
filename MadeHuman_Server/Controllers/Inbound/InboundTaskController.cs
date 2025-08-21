using MadeHuman_Server.Service.Inbound;
using Madehuman_Share.ViewModel.Inbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MadeHuman_Server.Controllers.Inbound
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class InboundTaskController : ControllerBase
    {
        private readonly IInboundTaskSvc _inboundTaskSvc;

        public InboundTaskController(IInboundTaskSvc inboundTaskService)
        {
            _inboundTaskSvc = inboundTaskService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateInboundTaskViewModel vm)
        {
            var userId = HttpContext.Items["User"]?.ToString(); // 👈 lấy từ middleware

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Không tìm thấy UserId từ JWT.");

            var result = await _inboundTaskSvc.CreateInboundTaskAsync(vm, userId);
            return Ok(new
            {
                isSuccess = true,
                message = "Inbound task created successfully.",
                data = result
            });
        }


        [HttpGet("{inboundTaskId}")]
        public async Task<IActionResult> GetById(Guid inboundTaskId)
        {
            var result = await _inboundTaskSvc.GetInboundTaskByIdAsync(inboundTaskId);
            if (result == null)
                return NotFound("Không tìm thấy InboundTask.");

            return Ok(result);
        }
        [HttpPost("validate-scan")]
        public async Task<IActionResult> ValidateScan([FromBody] ScanInboundTaskValidationRequest request)
        {
            var result = await _inboundTaskSvc.ValidateInboundProductScanAsync(request);

            // Nếu chỉ có 1 kết quả và là ✅ → trả 200 OK
            if (result.Count == 1 && result[0].StartsWith("✅"))
                return Ok(new { success = true, message = result[0] });

            // Nếu có nhiều lỗi → trả 400 BadRequest
            return BadRequest(new { success = false, errors = result });
        }
        [HttpGet]
        public async Task<ActionResult<List<InboundTaskViewModel>>> GetAll()
        {
            var result = await _inboundTaskSvc.GetAllAsync();
            return Ok(result);
        }
    }
}
