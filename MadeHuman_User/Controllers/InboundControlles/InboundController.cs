using MadeHuman_User.ServicesTask.Services.InboundService;
using MadeHuman_User.Models;
using Microsoft.AspNetCore.Mvc;
using Madehuman_Share.ViewModel.Inbound;
using MadeHuman_User.Helper;
using Madehuman_Share.ViewModel;


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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(Guid receiptId)
        {
            var success = await _inboundTaskService.CreateAsync(receiptId, HttpContext);

            if (!success)
            {
                ViewBag.Error = "❌ Tạo nhiệm vụ thất bại.";
                return View(); // Ở lại trang tạo khi lỗi
            }

            // Cho người dùng biết đang chuyển trang
            TempData["Success"] = "✅ Tạo nhiệm vụ nhập kho thành công. Đang chuyển sang trang quét xác nhận...";

            // 👉 Redirect sang ValidateScan, truyền receiptId để trang ValidateScan tự tra InboundTaskId
            return RedirectToAction(nameof(ValidateScan), new { receiptId });
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
        public async Task<IActionResult> ValidateScan(Guid? inboundTaskId = null, Guid? receiptId = null)
        {
            var vm = new InboundValidatePageViewModel
            {
                ScanRequest = new ScanInboundTaskValidationRequest()
            };

            // Nếu chưa có inboundTaskId nhưng có receiptId -> tra ID từ receipt
            if (!inboundTaskId.HasValue && receiptId.HasValue)
            {
                var foundId = await _inboundTaskService.GetTaskIdByReceiptAsync(receiptId.Value, HttpContext);
                if (foundId != Guid.Empty) inboundTaskId = foundId;
                else TempData["Error"] = "❌ Không tìm thấy InboundTask cho phiếu này.";
            }

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
            var data = await _inboundTaskService.GetAllAsync(token);
            var query = data.AsQueryable();

            // ----- Filter theo status -----
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals("Incomplete", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(x => !string.Equals(x.Status, "Completed", StringComparison.OrdinalIgnoreCase));
                else
                    query = query.Where(x => string.Equals(x.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            // ----- Filter theo từ khoá -----
            var q = searchTerm?.Trim();
            var qLower = q?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(qLower))
            {
                query = query.Where(x =>
                    x.Id.ToString().Contains(q!, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(x.CreateBy) && x.CreateBy!.ToLower().Contains(qLower))
                );
            }

            // ----- Thống kê sau lọc (cho phần "Stats") -----
            var totalAfterFilter = query.Count();
            var totalCompleted = query.Count(x => x.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase));
            var totalIncomplete = totalAfterFilter - totalCompleted;

            // ----- Paging -----
            if (pageSize <= 0 || pageSize > 200) pageSize = 6;
            var totalPages = (int)Math.Ceiling(totalAfterFilter / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = query
                .OrderByDescending(x => x.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new Madehuman_Share.ViewModel.PagedResult<InboundTaskViewModel>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalAfterFilter,
                TotalCompleted = totalCompleted,
                TotalIncomplete = totalIncomplete,
                Status = status,
                Q = searchTerm
            };

            // Pager SSR: giữ nguyên filter hiện tại
            ViewBag.Pagination = PaginationHelper.GeneratePagination(
                currentPage: vm.CurrentPage,
                totalPages: vm.TotalPages,
                baseUrl: Url.Action("Index", "Inbound")!,
                queryParams: new Dictionary<string, string>
                {
                    ["status"] = status ?? "",
                    ["searchTerm"] = searchTerm ?? "",
                    ["pageSize"] = pageSize.ToString()
                }
            );

            return View(vm);
        }



        [HttpGet]
        public async Task<IActionResult> Export(int page = 1, int pageSize = 6, string? status = "")
        {
            // Lấy nguồn dữ liệu
            var all = await _refillTaskService.GetAllRefillTasksAsync(HttpContext);

            // Lọc theo trạng thái (nếu có)
            IEnumerable<RefillTaskFullViewModel> q = all;
            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(t => string.Equals(t.StatusRefillTasks, status, StringComparison.OrdinalIgnoreCase));

            // Thống kê sau lọc (dùng biến tạm để tránh enumerate nhiều lần)
            var filtered = q.ToList();

            var totalAfterFilter = filtered.Count;
            var completedAfter = filtered.Count(t => string.Equals(t.StatusRefillTasks, "Completed", StringComparison.OrdinalIgnoreCase));
            var canceledAfter = filtered.Count(t => string.Equals(t.StatusRefillTasks, "Canceled", StringComparison.OrdinalIgnoreCase));
            var processingAfter = filtered.Count(t => string.Equals(t.StatusRefillTasks, "Incomplete", StringComparison.OrdinalIgnoreCase));
            var detailTotalAfter = filtered.Sum(t => t.Details?.Count ?? 0);

            // Chuẩn hoá pageSize & current page
            if (pageSize <= 0 || pageSize > 200) pageSize = 10;

            var totalPages = (int)Math.Ceiling(totalAfterFilter / (double)pageSize);
            if (totalPages < 1) totalPages = 1;

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            // Phân trang
            var items = filtered
                .OrderByDescending(t => t.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new PagedResult<RefillTaskFullViewModel>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalAfterFilter,
                Status = status,
                Q = null
            };

            ViewBag.TaskTotal = totalAfterFilter;
            ViewBag.TaskCompleted = completedAfter;
            ViewBag.TaskCanceled = canceledAfter;
            ViewBag.TaskProcessing = processingAfter;
            ViewBag.DetailTotal = detailTotalAfter;

            // 🔁 Sửa baseUrl: Inbound + giữ filter
            ViewBag.Pagination = PaginationHelper.GeneratePagination(
                currentPage: vm.CurrentPage,
                totalPages: vm.TotalPages,
                baseUrl: Url.Action("Export", "Inbound")!,   // <— đổi về Inbound
                queryParams: new Dictionary<string, string>
                {
                    ["status"] = status ?? "",
                    ["pageSize"] = pageSize.ToString()
                }
            );

            return View(vm); // Views/Inbound/Export.cshtml
        }



    }
}
