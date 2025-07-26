using MadeHuman_Server.Service.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Outbound
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PackTaskController : ControllerBase
    {
        private readonly IPackTaskService _packTaskService;

        public PackTaskController(IPackTaskService packTaskService)
        {
            _packTaskService = packTaskService;
        }

        /// <summary>
        /// Tạo một nhiệm vụ PackTask cho OutboundTaskItem
        /// </summary>
        //[HttpPost("create/{outboundTaskItemId}")]
        //public async Task<IActionResult> CreatePackTask(Guid outboundTaskItemId)
        //{
        //    try
        //    {
        //        var result = await _packTaskService.CreatePackTaskAsync(outboundTaskItemId);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        /// <summary>
        /// Gán người dùng vào PackTask và xử lý hoàn thành
        /// </summary>
        [HttpPost("assign/{outboundTaskItemId}")]
        public async Task<IActionResult> AssignPackTask(Guid outboundTaskItemId)
        {
            try
            {
                var logs = await _packTaskService.AssignPackTaskAsync(outboundTaskItemId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
