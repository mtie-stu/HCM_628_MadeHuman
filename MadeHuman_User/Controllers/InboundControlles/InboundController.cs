using MadeHuman_User.ServicesTask.Services.InboundService;
using MadeHuman_User.Models;
using Microsoft.AspNetCore.Mvc;
using Madehuman_Share.ViewModel.Inbound;


namespace MadeHuman_User.Controllers.InboundControlles
{
    public class InboundController : Controller
    {
        private readonly IInboundTaskService _inboundTaskService;
        private readonly IRefillTaskService _refillTaskService;


        public InboundController(IInboundTaskService inboundTaskService, IRefillTaskService refillTaskService)
        {
            _inboundTaskService = inboundTaskService;
            _refillTaskService = refillTaskService;
        }
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(Guid receiptId)
        {
            var success = await _inboundTaskService.CreateAsync(receiptId, HttpContext);

            if (!success)
            {
                ViewBag.Error = "❌ Tạo nhiệm vụ thất bại.";
                return View(); // Không redirect
            }

            ViewBag.Success = "✅ Tạo nhiệm vụ nhập kho thành công.";
            ViewBag.ReceiptId = receiptId; // Gửi ID nếu muốn sử dụng sau này
            return View(); // Giữ nguyên tại trang
        }

        [HttpGet]
        public IActionResult ValidateScan(Guid? inboundTaskId = null)
        {
            var vm = new ScanInboundTaskValidationRequest();

            if (inboundTaskId.HasValue)
                vm.InboundTaskId = inboundTaskId.Value; // gán vào ViewModel

            return View(vm); // truyền sang View => tự đổ vào input
        }


        [HttpPost]
        public async Task<IActionResult> ValidateScan(ScanInboundTaskValidationRequest request)
        {
            var (success, message, errors) = await _inboundTaskService.ValidateScanAsync(request, HttpContext);

            if (success)
                ViewBag.Success = message;
            else
                ViewBag.Errors = errors;

            return View(request);
        }
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["JWTToken"] ?? "";
            var tasks = await _inboundTaskService.GetAllAsync(token);
            return View(tasks);
        }

        // Trang nhập mã để quét task export
        [HttpGet]
        public async Task<IActionResult> Export()
        {
            // ✅ Truyền HttpContext
            var tasks = await _refillTaskService.GetAllRefillTasksAsync(HttpContext);
            return View(tasks);
        }
    }
}
