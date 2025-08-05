using MadeHuman_User.ServicesTask.Services.OutboundService;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Mvc;
using MadeHuman_User.ServicesTask.Services.Warehouse;
using MadeHuman_User.ServicesTask.Services.ShopService;
using System.Threading.Tasks;

namespace MadeHuman_User.Controllers.Outbound
{
    [Route("PickTasks")]
    public class PickTasksController : Controller
    {
        private readonly IPickTaskApiService _pickTaskService;
        private readonly ILogger<PickTasksController> _logger;
        private readonly IProductService _productLookup;
        private readonly IWarehouseLookupApiService _warehouseLookup;
        public PickTasksController(IPickTaskApiService pickTaskService, ILogger<PickTasksController> logger, IProductService productLookup,
    IWarehouseLookupApiService warehouseLookup)
        {
            _pickTaskService = pickTaskService;
            _logger = logger;
            _productLookup = productLookup;
            _warehouseLookup = warehouseLookup;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var tasks = await _pickTaskService.GetMyPickTasksAsync(HttpContext);
                return View("Index", tasks);
            }
            catch (HttpRequestException ex)
            {
                // Nếu lỗi từ phía API (như 500)
                TempData["Error"] = "Lỗi không tìm thấy mã phân công công việc của người dùng.";
                return View("PickTaskError"); // View tùy chỉnh
            }
            catch (Exception ex)
            {
                // Các lỗi khác
                TempData["Error"] = ex.Message;
                return View("PickTaskError");
            }
        }


        [HttpPost("assign")]
        public async Task<IActionResult> Assign()
        {
            var taskId = await _pickTaskService.AssignPickTaskAsync(HttpContext);

            if (taskId == null)
            {
                TempData["Error"] = "Không tìm thấy nhiệm vụ phù hợp.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Đã nhận nhiệm vụ thành công.";
            return RedirectToAction("scan-detail", "PickTasks", new { id = taskId });
        }


        [HttpGet("scan-detail/{id:guid}")] // ✅ route rõ ràng cho quét chi tiết
        public async Task<IActionResult> Scan(Guid id)
        {
            var viewModel = await _pickTaskService.GetPickTaskDetailAsync(HttpContext, id);
            if (viewModel?.BasketId == null)
            {
                TempData["NullBasket"] = "Hãy quét mã giỏ hàng trước khi bắt đầu.";
            }



            // Duyệt từng detail và gọi API để làm giàu dữ liệu
            foreach (var detail in viewModel.Details)
            {
                var skuInfo = await _productLookup.GetSKUInfoAsync(detail.ProductSKUId);
                if (skuInfo != null)
                {
                    detail.ProductName = skuInfo.ProductName;
                    detail.SkuCode = skuInfo.SkuCode;
                }

                var locationInfo = await _warehouseLookup.GetLocationInfoAsync(detail.WarehouseLocationId);
                if (locationInfo != null)
                {
                    detail.WarehouseLocationCode = locationInfo.NameLocation;
                }
            }

            return View(viewModel);
        }


        [HttpPost("scan")]
        public async Task<IActionResult> ValidateScan([FromForm] ScanPickTaskValidationRequest request)
        {
            var result = await _pickTaskService.ValidatePickTaskScanAsync(HttpContext, request);
            TempData["ScanResult"] = result;
            return RedirectToAction("Scan");
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm(Guid pickTaskId, Guid pickTaskDetailId, Guid basketId)
        {
            var result = await _pickTaskService.ConfirmPickDetailToBasketAsync(HttpContext, pickTaskId, pickTaskDetailId, basketId);
            TempData["ConfirmResult"] = result;
            return RedirectToAction("Scan");
        }
        [HttpGet("scan-basket/{id:guid}")] // ✅ route rõ ràng cho quét rổ
        public IActionResult ScanBasket(Guid id)
        {
            var model = new ConfirmBasketRequest
            {
                PickTaskId = id
            };
            return View(model); // Views/PickTask/ScanBasket.cshtml
        }



        [HttpGet("scan-pick-detail/{id:guid}")]
        public async Task<IActionResult> ScanPickDetail(Guid id)
        {
            var viewModel = await _pickTaskService.GetPickTaskDetailAsync(HttpContext, id);
            if (viewModel == null)
            {
                TempData["Error"] = "Không tìm thấy nhiệm vụ Pick tương ứng.";
                return RedirectToAction("Index"); // hoặc quay về danh sách PickTask
            }

            var detail = viewModel.Details.FirstOrDefault();
            if (detail != null)
            {
                var skuInfo = await _productLookup.GetSKUInfoAsync(detail.ProductSKUId);
                if (skuInfo != null)
                {
                    detail.ProductName = skuInfo.ProductName;
                    detail.SkuCode = skuInfo.SkuCode;
                    detail.ImageUrls = skuInfo.ImageUrls;
                }
                                                                        
                var locationInfo = await _warehouseLookup.GetLocationInfoAsync(detail.WarehouseLocationId);
                if (locationInfo != null)
                {
                    detail.WarehouseLocationCode = locationInfo.NameLocation;
                }
            }

            return View("ScanPickDetail", viewModel); // View đúng tên file
        }

        [HttpPost("scan-basket")]
        public async Task<IActionResult> ScanBasket(ConfirmBasketRequest model)
        {
            var result = await _pickTaskService.ConfirmBasketAsync(HttpContext, model);
            if (result.Any(m => m.StartsWith("❌")))
            {
                TempData["Error"] = string.Join("<br/>", result);
                return View(model);
            }

            TempData["Success"] = "✅ Xác nhận Basket thành công!";
            return RedirectToAction("Scan", new { id = model.PickTaskId });
        }

    }
}
