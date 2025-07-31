using Madehuman_Share.ViewModel.Inbound;
using MadeHuman_User.ServicesTask.Services.InboundService;
using MadeHuman_User.ServicesTask.Services.Warehouse;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers.InboundControlles
{
    public class RefillTaskController : Controller
    {
        private readonly IRefillTaskService _refillTaskService;
        private readonly IWarehouseLookupApiService _warehouseLocationService;

        public RefillTaskController(IRefillTaskService refillTaskService, IWarehouseLookupApiService warehouseLocationService)
        {
            _refillTaskService = refillTaskService;
            _warehouseLocationService = warehouseLocationService;
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
        //[HttpGet]
        //public IActionResult ValidateScan()
        //{
        //    // Trả về view với model rỗng để hiển thị form nhập
        //    return View(new ScanRefillTaskValidationRequest());
        //}
        [HttpGet]
        public async Task<IActionResult> ValidateScan(
            Guid refillTaskId,
            Guid refillTaskDetailId,
            Guid fromLocation,
            Guid toLocation,
            string sku,
            int quantity,
            string? createBy,
            string? createAt)
        {
            var fromLocationInfo = await _warehouseLocationService.GetLocationInfoAsync(fromLocation);
            var toLocationInfo = await _warehouseLocationService.GetLocationInfoAsync(toLocation);

            var detail = new RefillTaskDetailWithHeaderViewModel
            {
                RefillTaskId = refillTaskId,
                DetailId = refillTaskDetailId,
                FromLocation = fromLocation,
                ToLocation = toLocation,
                SKU = sku,
                Quantity = quantity,
                CreateBy = createBy,
                CreateAt = DateTime.TryParse(createAt, out var parsedDate) ? parsedDate : DateTime.UtcNow
            };

            var vm = new RefillScanPageViewModel
            {
                TaskDetailFlat = detail,
                ScanRequest = new ScanRefillTaskValidationRequest
                {
                    RefillTaskId = refillTaskId,
                    RefillTaskDetailId = refillTaskDetailId,
                    FromLocationName = fromLocationInfo?.NameLocation ?? fromLocation.ToString(),
                    ToLocationName = toLocationInfo?.NameLocation ?? toLocation.ToString(),
                    SKU = sku,
                    Quantity = quantity
                }
            };

            return View(vm);
        }

        //[HttpPost]
        //public async Task<IActionResult> ValidateScan(ScanRefillTaskValidationRequest request)
        //{
        //    var messages = await _refillTaskService.ValidateRefillScanAsync(request, HttpContext);

        //    if (messages.Any(m => m.Contains("✅")))
        //        ViewBag.Success = string.Join("<br/>", messages);
        //    else
        //        ViewBag.Errors = messages;

        //    // 🔁 Load lại thông tin vị trí + thời gian tạo
        //    var fromInfo = await _warehouseLocationService.GetLocationInfoByNameAsync(request.FromLocationName);
        //    var toInfo = await _warehouseLocationService.GetLocationInfoByNameAsync(request.ToLocationName);

        //    var detail = new RefillTaskDetailWithHeaderViewModel
        //    {
        //        RefillTaskId = request.RefillTaskId,
        //        DetailId = request.RefillTaskDetailId,
        //        FromLocation = fromInfo?.Id ?? Guid.Empty,
        //        ToLocation = toInfo?.Id ?? Guid.Empty,
        //        SKU = request.SKU,
        //        Quantity = request.Quantity ?? 0,
        //        CreateAt = DateTime.UtcNow // bạn có thể lưu thời gian gốc nếu muốn
        //    };

        //    var vm = new RefillScanPageViewModel
        //    {
        //        TaskDetailFlat = detail,
        //        ScanRequest = request
        //    };

        //    return View(vm); // ✅ đúng model
        //}


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
