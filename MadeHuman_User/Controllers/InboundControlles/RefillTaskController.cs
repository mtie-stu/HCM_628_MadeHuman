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
            var tasks = await _refillTaskService.GetAllRefillTasksAsync(HttpContext);
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
            var success = await _refillTaskService.CreateRefillTaskAsync(model, HttpContext);

            if (success)
            {
                TempData["Success"] = "✅ Tạo nhiệm vụ Refill thành công.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "❌ Không thể tạo nhiệm vụ Refill.";
            return View(model);
        }

    }
}
