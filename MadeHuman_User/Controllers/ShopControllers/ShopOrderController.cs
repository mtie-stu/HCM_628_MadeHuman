using Madehuman_Share.ViewModel.Shop;
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
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
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
