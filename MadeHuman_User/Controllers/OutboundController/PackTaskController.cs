using MadeHuman_User.ServicesTask.Services.OutboundService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers.OutboundController
{
    public class PackTaskController : Controller
    {
        private readonly IPackTaskService _apiClient;

        public PackTaskController(IPackTaskService apiClient)
        {
            _apiClient = apiClient;
        }
        // GET: /PackTask/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Trả về Razor View: Views/PackTask/Index.cshtml
        }
        [HttpPost]
        public async Task<IActionResult> Assign(Guid outboundTaskItemId)
        {
            var result = await _apiClient.AssignPackTaskAsync(outboundTaskItemId, HttpContext);

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
