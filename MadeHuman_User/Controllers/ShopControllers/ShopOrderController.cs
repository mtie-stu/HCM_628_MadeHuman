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
            return View(new CreateShopOrderWithMultipleItems
            {
                Items = new List<OrderItemInputModel>
                {
                    new OrderItemInputModel()
                }
                });
            }

        [HttpPost]
        public async Task<IActionResult> Create(CreateShopOrderWithMultipleItems model)
        {
            try
            {
                var success = await _orderService.CreateOrderAsync(model);

                if (success)
                    return RedirectToAction("Index");

                ModelState.AddModelError("", "Tạo đơn hàng thất bại.");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi controller: " + ex.Message);
                ModelState.AddModelError("", "Lỗi không xác định.");
                return View(model);
            }
        }


    }
}
