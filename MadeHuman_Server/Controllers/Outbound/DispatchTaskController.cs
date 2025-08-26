using MadeHuman_Server.Service.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Outbound
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DispatchTaskController : ControllerBase
    {
        private readonly IDispatchTaskServices _dispatchTaskService;

        public DispatchTaskController(IDispatchTaskServices dispatchTaskService)
        {
            _dispatchTaskService = dispatchTaskService;
        }

        /// <summary>
        /// Tạo DispatchTask cho một OutboundTaskItem
        /// </summary>
        //[HttpPost("create/{outboundTaskItemId}")]
        //public async Task<IActionResult> CreateDispatchTask(Guid outboundTaskItemId)
        //{
        //    try
        //    {
        //        var result = await _dispatchTaskService.CreateDisPactchPackTaskAsync(outboundTaskItemId);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        /// <summary>
        /// Gán người dùng vào DispatchTask và xử lý hoàn tất
        /// </summary>
        [HttpPost("assign/{outboundTaskItemId}")]
        public async Task<IActionResult> AssignDispatchTask(Guid outboundTaskItemId)
        {
            try
            {
                var logs = await _dispatchTaskService.AssignDisPactchTaskAsync(outboundTaskItemId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
