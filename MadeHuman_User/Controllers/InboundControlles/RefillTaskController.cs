using Madehuman_Share.ViewModel.Inbound;
using MadeHuman_User.ServicesTask.Services.InboundService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MadeHuman_User.Controllers.InboundControlles
{
    public class RefillTaskController : Controller
    {
        private readonly IRefillTaskService _refillTaskService;

        public RefillTaskController(IRefillTaskService refillTaskService)
        {
            _refillTaskService = refillTaskService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tasks = await _refillTaskService.GetAllRefillTasksAsync();
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var email = Request.Cookies["EmailOrId"];
            return View(new RefillTaskFullViewModel
            {
                CreateBy = email
            });
        }


        [HttpPost]
        public async Task<IActionResult> Create(RefillTaskFullViewModel model)
        {
            var success = await _refillTaskService.CreateRefillTaskAsync(model);
            if (success)
            {
                TempData["Success"] = "✅ Tạo nhiệm vụ bổ sung thành công!";
                return RedirectToAction("Create");
            }

            TempData["Error"] = "❌ Lỗi khi tạo nhiệm vụ bổ sung!";
            return View(model);
        }
    }
}
