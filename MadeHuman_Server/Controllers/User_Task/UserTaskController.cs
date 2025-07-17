using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.PartTime_Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.User_Task
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskSvc _userTaskSvc;

        public UserTaskController(IUserTaskSvc userTaskSvc)
        {
            _userTaskSvc = userTaskSvc;
        }

        [HttpPost("checkin-checkout")]
        public async Task<IActionResult> CheckinCheckoutAsync([FromBody] Checkin_Checkout_Viewmodel request)
        {
            try
            {
                var model = new Checkin_Checkout_Viewmodel
                {
                    PartTimeId = request.PartTimeId,
                    UserId = request.UserId,
                    Note = request.Note,
                    BreakDuration = request.BreakDuration
                };

                var result = await _userTaskSvc.Checkin_Checkout_Async(model, request.regime);
                return Ok(new
                {
                    isSuccess = true,
                    message = "Thao tác thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayLogs()
        {
            var result = await _userTaskSvc.GetCheckInOutTodayAsync();
            return Ok(result);
        }

    }
}
