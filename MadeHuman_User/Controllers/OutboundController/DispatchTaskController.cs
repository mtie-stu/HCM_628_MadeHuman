using MadeHuman_User.ServicesTask.Services.OutboundService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers.OutboundController
{
    public class DispatchTaskController : Controller
    {
        private readonly IDispatchTaskService _apiClient;

        public DispatchTaskController(IDispatchTaskService apiClient)
        {
            _apiClient = apiClient;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Assign(Guid outboundTaskItemId)
        {
            var result = await _apiClient.AssignDispatchTaskAsync(outboundTaskItemId, HttpContext);

            if (result == null)
            {
                TempData["Logs"] = "❌ Gọi API thất bại hoặc không có JWT.";
            }
            else
            {
                TempData["Logs"] = string.Join("<br>", result.Logs);
            }

            return RedirectToAction("Index");
        }
    }
}
