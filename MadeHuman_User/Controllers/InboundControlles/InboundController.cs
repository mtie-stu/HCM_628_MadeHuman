using MadeHuman_User.ServicesTask.Services.InboundService;
using MadeHuman_User.Models;
using Microsoft.AspNetCore.Mvc;
using Madehuman_Share.ViewModel.Inbound;
using MadeHuman_User.Helper;


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

        //[HttpGet]
        //public IActionResult ValidateScan(Guid? inboundTaskId = null)
        //{
        //    var vm = new ScanInboundTaskValidationRequest();

        //    if (inboundTaskId.HasValue)
        //        vm.InboundTaskId = inboundTaskId.Value; // gán vào ViewModel

        //    return View(vm); // truyền sang View => tự đổ vào input
        //}


        //[HttpPost]
        //public async Task<IActionResult> ValidateScan(ScanInboundTaskValidationRequest request)
        //{
        //    var (success, message, errors) = await _inboundTaskService.ValidateScanAsync(request, HttpContext);

        //    if (success)
        //        ViewBag.Success = message;
        //    else
        //        ViewBag.Errors = errors;

        //    return View(request);
        //}
        [HttpGet]
        public async Task<IActionResult> ValidateScan(Guid? inboundTaskId = null)
        {
            var vm = new InboundValidatePageViewModel { ScanRequest = new ScanInboundTaskValidationRequest() };

            if (inboundTaskId.HasValue)
            {
                vm.ScanRequest.InboundTaskId = inboundTaskId.Value;
                vm.TaskInfo = await _inboundTaskService.GetByIdAsync(inboundTaskId.Value, HttpContext);
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateScan(InboundValidatePageViewModel vm)
        {
            var (success, message, errors) =
                await _inboundTaskService.ValidateScanAsync(vm.ScanRequest, HttpContext);

            if (success) ViewBag.Success = message;
            else ViewBag.Errors = errors;

            // ✅ Luôn load lại thông tin InboundTask
            if (vm.ScanRequest.InboundTaskId != Guid.Empty)
            {
                vm.TaskInfo = await _inboundTaskService.GetByIdAsync(vm.ScanRequest.InboundTaskId, HttpContext);

                // 👉 Nếu tất cả ProductBatches đã có trạng thái "Store" thì redirect về Index
                if (vm.TaskInfo != null
                    && vm.TaskInfo.ProductBatches != null
                    && vm.TaskInfo.ProductBatches.All(b => b.StatusProductBatch.Equals("Stored", StringComparison.OrdinalIgnoreCase)))
                {
                    // Có thể set TempData để báo thành công
                    TempData["Success"] = "✅ Tất cả lô đã được lưu kho thành công.";
                    return RedirectToAction("Index", "Inbound");
                }
            }

            return View(vm);
        }



        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 6,
            string? status = "",
            string? searchTerm = "")
        {
            var token = Request.Cookies["JWTToken"] ?? "";
            var allTasks = await _inboundTaskService.GetAllAsync(token);

            // >>> THÊM: Lưu lại giá trị filter để view còn dùng
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = searchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                allTasks = allTasks.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    (x.CreateBy ?? "").ToLower().Contains(searchTerm)
                ).ToList();
            }

            // filter server-side
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Incomplete")
                    allTasks = allTasks.Where(x => x.Status != "Completed").ToList();
                else
                    allTasks = allTasks.Where(x => x.Status == status).ToList();
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                allTasks = allTasks.Where(x =>
                    x.Id.ToString().Contains(searchTerm) ||
                    (x.CreateBy ?? "").ToLower().Contains(searchTerm)
                ).ToList();
            }

            int totalCount = allTasks.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = allTasks
                .OrderByDescending(x => x.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // >>> SỬA: Pagination luôn gắn kèm filter hiện tại
            ViewBag.Pagination = PaginationHelper.GeneratePagination(
                page,
                totalPages,
                Url.Action("Index", "Inbound")!,
                new Dictionary<string, string>
                {
                    ["status"] = status ?? "",
                    ["searchTerm"] = searchTerm ?? ""
                }
            );

            return View(items);
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
