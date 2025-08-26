using Madehuman_Share.ViewModel.Inbound;
using MadeHuman_Admin.ServicesTask.Services.InboundService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Admin.Controllers.InboundControlles
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
            // ✅ Truyền HttpContext
            var tasks = await _refillTaskService.GetAllRefillTasksAsync(HttpContext);
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var email = Request.Cookies["EmailOrId"]; // 👈 Nếu không có thì gán "" hoặc null
            return View(new RefillTaskFullViewModel
            {
                CreateBy = email ?? ""
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(RefillTaskFullViewModel model)
        {
            var success = await _refillTaskService.CreateRefillTaskAsync(model, HttpContext); // ✅ Truyền HttpContext

            if (success)
            {
                TempData["Success"] = "✅ Tạo nhiệm vụ bổ sung thành công!";
                return RedirectToAction("Create");
            }

            TempData["Error"] = "❌ Lỗi khi tạo nhiệm vụ bổ sung!";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var task = await _refillTaskService.GetByIdAsync(id, HttpContext); // ✅ Truyền HttpContext
            if (task == null)
            {
                TempData["Error"] = "❌ Không tìm thấy nhiệm vụ.";
                return RedirectToAction("Index");
            }

            return View(task);
        }
        public async Task<IActionResult> DetailFlat()
        {
            var data = await _refillTaskService.GetAllDetailsAsync(HttpContext);
            return View(data);
        }
        [HttpGet]
        public IActionResult ValidateScan()
        {
            // Trả về view với model rỗng để hiển thị form nhập
            return View(new ScanRefillTaskValidationRequest());
        }

        [HttpPost]
        public async Task<IActionResult> ValidateScan(ScanRefillTaskValidationRequest request)
        {
            // Gọi service xử lý xác nhận bổ sung
            var messages = await _refillTaskService.ValidateRefillScanAsync(request, HttpContext);

            // Phân loại kết quả: thành công hay lỗi
            if (messages.Any(m => m.Contains("✅")))
            {
                ViewBag.Success = string.Join("<br/>", messages);
            }
            else
            {
                ViewBag.Errors = messages;
            }

            // Trả lại view với model đã nhập
            return View(request);
        }


    }
}
