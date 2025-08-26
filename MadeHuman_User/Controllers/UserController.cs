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
        public async Task<IActionResult> CheckInOut()
        {
            var viewModel = new CheckinPageViewModel
            {
                Form = new Checkin_Checkout_Viewmodel(),
                TodayLogs = await _checkinService.GetTodayLogsAsync() // Tùy UserId nếu cần lọc
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CheckInOut(CheckinPageViewModel pageModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin.";
                pageModel.TodayLogs = await _checkinService.GetTodayLogsAsync();
                return View(pageModel);
            }

            var isSuccess = await _checkinService.SubmitCheckinCheckoutAsync(pageModel.Form);

            if (isSuccess)
            {
                TempData["Success"] = "Checkin/Checkout thành công.";
                return RedirectToAction(nameof(CheckInOut));
            }

            TempData["Error"] = "Gửi dữ liệu thất bại.";
            pageModel.TodayLogs = await _checkinService.GetTodayLogsAsync();
            return View(pageModel);
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
