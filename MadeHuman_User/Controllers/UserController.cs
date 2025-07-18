using Madehuman_User.ViewModel.PartTime_Task;
using MadeHuman_User.ServicesTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    public class UserController : Controller
    {
        private readonly ICheckinCheckoutService _checkinService;

        public UserController(ICheckinCheckoutService checkinService)
        {
            _checkinService = checkinService;
        }
        [HttpGet]
        public IActionResult CheckInOut()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckInOut(Checkin_Checkout_Viewmodel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin.";
                return View(model);
            }

            var isSuccess = await _checkinService.SubmitCheckinCheckoutAsync(model);

            if (isSuccess)
            {
                TempData["Success"] = "Checkin/Checkout thành công.";
                return RedirectToAction(nameof(CheckInOut));
            }

            TempData["Error"] = "Gửi dữ liệu thất bại.";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AllTodayLogs()
        {
            var logs = await _checkinService.GetTodayLogsAsync();
            return View(logs);
        }
        public IActionResult Reports()
        {
            return View();
        }
    }
}
