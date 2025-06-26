using MadeHuman_Server.Model.User_Task;
using Madehuman_Share.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumController : ControllerBase
    {
       
        [HttpGet("status-parttime")]
            public IActionResult GetStatusPartTime()
            {
                var result = Enum.GetValues(typeof(StatusPartTime))
                    .Cast<StatusPartTime>()
                    .Select(e => new EnumOptionViewModel
                    {
                        Value = (int)e,
                        Name = e.ToString()
                    }).ToList();

                return Ok(result);
            }

            [HttpGet("status-company")]
            public IActionResult GetStatusCompany()
            {
                var result = Enum.GetValues(typeof(StatusPart_Time_Company))
                    .Cast<StatusPart_Time_Company>()
                    .Select(e => new EnumOptionViewModel
                    {
                        Value = (int)e,
                        Name = e.ToString()
                    }).ToList();

                return Ok(result);
            }

            [HttpGet("task-type")]
            public IActionResult GetTaskType()
            {
                var result = Enum.GetValues(typeof(TaskType))
                    .Cast<TaskType>()
                    .Select(e => new EnumOptionViewModel
                    {
                        Value = (int)e,
                        Name = e.ToString()
                    }).ToList();

                return Ok(result);
            }
    }
}
