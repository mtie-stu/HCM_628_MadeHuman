using MadeHuman_Server.Service.Inbound;
using Madehuman_Share.ViewModel.Inbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Controllers.Inbound
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RefillTaskController : ControllerBase
    {
        private readonly IRefillTaskService _refillTaskService;

        public RefillTaskController(IRefillTaskService refillTaskService)
        {
            _refillTaskService = refillTaskService;
        }

        // GET: api/RefillTask
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _refillTaskService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/RefillTask/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _refillTaskService.GetByIdAsync(id);
            if (result == null)
                return NotFound("❌ Không tìm thấy RefillTask");

            return Ok(result);
        }

        // POST: api/RefillTask

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RefillTaskFullViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("❌ Không xác định được người dùng");

            var result = await _refillTaskService.CreateAsync(model, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/RefillTask/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RefillTaskFullViewModel model)
        {
            var existing = await _refillTaskService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("❌ Không tìm thấy RefillTask để cập nhật");

            var result = await _refillTaskService.UpdateAsync(id, model);
            return Ok(result);
        }
        [HttpGet("Alldetails")]
        public async Task<IActionResult> GetAllDetails()
        {
            var result = await _refillTaskService.GetAllDetailsAsync();
            return Ok(result);
        }
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRefillTask()
        {
            try
            {
                var task = await _refillTaskService.AssignRefillTaskToCurrentUserAsync();

                if (task == null)
                    return NotFound("🎉 Hiện không còn nhiệm vụ nào chưa được nhận.");

                return Ok(new { taskId = task.Id }); // 🔁 CHỈ TRẢ ID
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"🚨 Lỗi hệ thống: {ex.Message}");
            }
        }
        [HttpPost("validate-scan")]
        public async Task<IActionResult> ValidateScan([FromBody] ScanRefillTaskValidationRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Yêu cầu không hợp lệ.");

                var result = await _refillTaskService.ValidateRefillTaskScanAsync(request);
                return Ok(result);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



    }
}
