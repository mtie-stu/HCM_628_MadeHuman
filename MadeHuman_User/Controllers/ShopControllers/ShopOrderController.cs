using Madehuman_User.ViewModel.Shop;
using MadeHuman_User.ServicesTask.Services.ShopService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers.ShopControllers
{
    public class ShopOrderController : Controller
    {
        private readonly IShopOrderService _orderService;

        public ShopOrderController(IShopOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6, string? q = "", string? status = "")
        {
            if (page < 1) page = 1;
            pageSize = (pageSize is < 1 or > 100) ? 6 : pageSize;

            // Lấy toàn bộ đơn
            var all = await _orderService.GetAllAsync() ?? new List<ShopOrderListItemViewModel>();

            // Danh sách trạng thái (để render dropdown)
            var statuses = all
                .Select(o => o.Status)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            // ----- Lọc theo trạng thái -----
            if (!string.IsNullOrWhiteSpace(status))
            {
                var st = status.Trim();
                all = all
                    .Where(o => string.Equals(o.Status ?? "", st, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // ----- Tìm kiếm theo từ khoá -----
            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.Trim().ToLowerInvariant();
                all = all.Where(o =>
                        (!string.IsNullOrEmpty(o.AppUserName) && o.AppUserName.ToLower().Contains(ql)) ||
                        (!string.IsNullOrEmpty(o.Status) && o.Status.ToLower().Contains(ql)) ||
                        o.ShopOrderId.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            // ----- Phân trang -----
            var totalItems = all.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            if (page > totalPages) page = totalPages;

            var data = all
                .OrderByDescending(o => o.OrderDate)       // sắp xếp mới nhất trước
                .ThenByDescending(o => o.ShopOrderId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBags phục vụ View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.Q = q ?? "";
            ViewBag.Status = status ?? "";
            ViewBag.Statuses = statuses;

            ViewBag.TotalItems = totalItems;
            ViewBag.FromItem = totalItems == 0 ? 0 : (page - 1) * pageSize + 1;
            ViewBag.ToItem = Math.Min(page * pageSize, totalItems);

            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new CreateShopOrderWithMultipleItems
            {
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItemInputModel> { new OrderItemInputModel() } // ít nhất 1 dòng để render
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateShopOrderWithMultipleItems model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin hợp lệ.";
                return View(model);
            }

            try
            {
                var response = await _orderService.CreateOrderAsync(model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "✅ Tạo đơn hàng thành công!";
                    return RedirectToAction("Index"); // hoặc trang danh sách đơn hàng
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("🔴 Lỗi API trả về:");
                Console.WriteLine(errorContent);

                TempData["Error"] = $"Tạo đơn hàng thất bại: {(int)response.StatusCode} - {response.ReasonPhrase}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                Console.WriteLine("🔴 Exception:");
                Console.WriteLine(ex);
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound("Không tìm thấy đơn hàng.");

            return View(order);
        }
    }
}
