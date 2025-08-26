using Madehuman_Share.ViewModel;
using Madehuman_Share.ViewModel.Inbound;
using Madehuman_Share.ViewModel.Shop;
using MadeHuman_User.Helper;
using MadeHuman_User.ServicesTask.Services.InboundService;
using MadeHuman_User.ServicesTask.Services.ShopService;
using MadeHuman_User.ServicesTask.Services.Warehouse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MadeHuman_User.Controllers.InboundControlles
{
    public class RefillTaskController : Controller
    {
        private readonly IRefillTaskService _refillTaskService;
        private readonly IWarehouseLookupApiService _warehouseLocationService;
        private readonly IProductService _productService;

        public RefillTaskController(IRefillTaskService refillTaskService, IWarehouseLookupApiService warehouseLocationService, IProductService productService)
        {
            _refillTaskService = refillTaskService;
            _productService = productService;
            _warehouseLocationService = warehouseLocationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var all = await _refillTaskService.GetAllRefillTasksAsync(HttpContext);

            // Thống kê tổng (cho stat cards)
            var total = all.Count;
            var completed = all.Count(t => string.Equals(t.StatusRefillTasks, "Completed", StringComparison.OrdinalIgnoreCase));
            var canceled = all.Count(t => string.Equals(t.StatusRefillTasks, "Canceled", StringComparison.OrdinalIgnoreCase));
            var processing = all.Count(t => string.Equals(t.StatusRefillTasks, "Incomplete", StringComparison.OrdinalIgnoreCase));

            // Danh sách hiển thị: chỉ "Đang xử lý" (giống view cũ)
            var q = all.Where(t => string.Equals(t.StatusRefillTasks, "Incomplete", StringComparison.OrdinalIgnoreCase));

            // Phân trang
            if (pageSize <= 0 || pageSize > 200) pageSize = 10;
            var totalAfter = q.Count(); // số "Đang xử lý"
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalAfter / (double)pageSize));
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = q
                .OrderByDescending(t => t.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewModel chuẩn PagedResult
            var vm = new PagedResult<RefillTaskFullViewModel>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalAfter,     // tổng "Đang xử lý" (sau lọc để list)
                Status = "Incomplete",   // ghi nhớ filter đang dùng
                Q = null,
                // Các field thống kê optional
                TotalCompleted = completed,
                TotalIncomplete = processing
            };

            // Đẩy số liệu cho stat cards
            ViewBag.TaskTotal = total;
            ViewBag.TaskCompleted = completed;
            ViewBag.TaskCanceled = canceled;
            ViewBag.TaskProcessing = processing;

            // Pager giữ pageSize (nếu cần thêm filter sau này thì add vào queryParams)
            ViewBag.Pagination = PaginationHelper.GeneratePagination(
                currentPage: vm.CurrentPage,
                totalPages: vm.TotalPages,
                baseUrl: Url.Action("Index", "RefillTask")!,
                queryParams: new Dictionary<string, string>
                {
                    ["pageSize"] = pageSize.ToString()
                }
            );

            return View(vm);
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
        [HttpGet]
        public async Task<IActionResult> DetailFlat(int page = 1, int pageSize = 4)
        {
            // Lấy toàn bộ chi tiết
            var all = await _refillTaskService.GetAllDetailsAsync(HttpContext);

            // ==== Stats tổng cho header ====
            var total = all.Count;
            var done = all.Count(x => x.IsRefilled);
            var pending = total - done;
            var groupsQuery = all
                .GroupBy(x => x.RefillTaskId)
                .OrderByDescending(g => g.Max(i => i.CreateAt));

            var totalGroups = groupsQuery.Count();

            // ==== Phân trang theo GROUP (RefillTaskId) ====
            if (pageSize <= 0 || pageSize > 200) pageSize = 5;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalGroups / (double)pageSize));
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pageGroupKeys = groupsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => g.Key)
                .ToHashSet();

            // Chỉ gửi lên View các item thuộc các nhóm ở trang hiện tại
            var pageItems = all
                .Where(x => pageGroupKeys.Contains(x.RefillTaskId))
                .ToList();

            // ==== ViewBags ====
            ViewBag.TotalRecords = total;
            ViewBag.TotalDone = done;
            ViewBag.TotalPending = pending;
            ViewBag.TotalGroups = totalGroups;

            // Pager giữ pageSize
            ViewBag.Pagination = PaginationHelper.GeneratePagination(
                currentPage: page,
                totalPages: totalPages,
                baseUrl: Url.Action("DetailFlat", "RefillTask")!, // đổi Controller nếu khác
                queryParams: new Dictionary<string, string>
                {
                    ["pageSize"] = pageSize.ToString()
                }
            );

            return View(pageItems);
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
            string? createAt,
            Guid? productSKUId)
        {
            var fromLocationInfo = await _warehouseLocationService.GetLocationInfoAsync(fromLocation);
            var toLocationInfo = await _warehouseLocationService.GetLocationInfoAsync(toLocation);
            var task = await _refillTaskService.GetByIdAsync(refillTaskId, HttpContext);

            // hoặc cách bạn lấy toàn bộ task
            var currentDetail = task.Details.FirstOrDefault(x => x.Id == refillTaskDetailId);
            // ✅ gọi lấy thông tin sản phẩm nếu có productSKUId
            ProductSKUInfoViewmodel? productInfo = null;
            if (currentDetail?.ProductSKUId != null)
            {
                productInfo = await _productService.GetSKUInfoAsync(currentDetail.ProductSKUId.Value);
            }
            var detail = new RefillTaskDetailWithHeaderViewModel
            {
                RefillTaskId = refillTaskId,
                DetailId = refillTaskDetailId,
                FromLocationName = fromLocationInfo?.NameLocation,
                ToLocationName = toLocationInfo?.NameLocation,
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
                    FromLocationName = fromLocationInfo?.NameLocation ?? "(Không rõ)",
                    ToLocationName = toLocationInfo?.NameLocation ?? "(Không rõ)",
                    SKU = sku,
                    Quantity = quantity
                },
                ProductInfo = productInfo // 👈 truyền thêm thông tin sản phẩm
            };

            return View(vm);
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
        [HttpPost]
        public async Task<IActionResult> Assign(CancellationToken ct)
        {
            var (ok, taskId, status, msg) = await _refillTaskService.AssignAsync(HttpContext, ct);

            if (!ok)
            {
                if (status == StatusCodes.Status401Unauthorized)
                    return Unauthorized(msg ?? "Bạn chưa đăng nhập.");
                if (status == StatusCodes.Status404NotFound)
                    return NotFound(msg ?? "Không còn nhiệm vụ nào để nhận.");
                return StatusCode(status == 0 ? 500 : status, msg ?? "Lỗi không xác định.");
            }

            // Chuẩn hoá: luôn trả JSON có taskId (nếu BE có trả)
            if (taskId.HasValue) return Ok(new { taskId = taskId.Value });

            // fallback nếu BE trả success mà không có taskId
            return Ok(new { message = msg ?? "Nhận nhiệm vụ thành công." });
        }
    }
}
