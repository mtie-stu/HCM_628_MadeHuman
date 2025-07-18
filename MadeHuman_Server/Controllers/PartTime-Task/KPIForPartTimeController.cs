using MadeHuman_Server.Service.UserTask;
using Madehuman_User.ViewModel.PartTime_Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.PartTime_Task
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIForPartTimeController : ControllerBase
    {
        private readonly IUserTaskSvc _taskService;

        public KPIForPartTimeController(IUserTaskSvc taskService)
        {
            _taskService = taskService;
        }
        /// <summary>
        /// Lấy danh sách KPI theo ngày và loại công việc
        /// </summary>
        /// <param name="workDate">Ngày làm việc (yyyy-MM-dd)</param>
        /// <param name="taskType">Loại công việc (Picker, Checker, Packer, Dispatcher)</param>
        [HttpGet("kpi")]
        public async Task<IActionResult> GetUserTaskKPI([FromQuery] DateOnly workDate, [FromQuery] TaskTypeUservm taskType)
        {
            var result = await _taskService.GetUserTaskSummariesAsync(workDate, taskType);
            return Ok(result);
        }
    }
}
