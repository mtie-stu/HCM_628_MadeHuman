using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Service.Shop;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MadeHuman_Server.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopOrderController : ControllerBase
    {
        private readonly IShopOrderService _orderService;

        public ShopOrderController(IShopOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();

            var result = orders.Select(o => new ShopOrderListItemViewModel
            {
                ShopOrderId = o.ShopOrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(), // Enum → string
                AppUserName = o.AppUser?.Name ?? "(Ẩn danh)",
                ItemCount = o.OrderItems?.Count ?? 0
            });

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            var viewModel = new ShopOrderDetailViewModel
            {
                ShopOrderId = order.ShopOrderId,
                AppUserId = order.AppUserId,
                AppUserEmail = order.AppUser?.Email,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = (Madehuman_Share.ViewModel.Shop.StatusOrder)order.Status,
                Items = order.OrderItems.Select(item => new OrderItemViewModel
                {
                    ProductSKUsId = item.ProductSKUsId,
                    ProductSKUCode = item.ProductSKUs?.SKU,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            return Ok(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShopOrderWithMultipleItems model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = order.ShopOrderId }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ShopOrder model)
        {
            var result = await _orderService.UpdateAsync(id, model);
            if (!result)
                return NotFound(new { message = "Không thể cập nhật đơn hàng." });

            return Ok(new { message = "Cập nhật thành công." });
        }

  

        [HttpPost("generate-single-product")]
        public async Task<IActionResult> GenerateRandomSingleProduct([FromBody] GenerateRandomOrdersSingleRequest model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _orderService.CreateRandomOrdersWithSingleProductAsync(model,userId);
            return Ok(new
            {
                message = $"Tạo {result.Count} đơn hàng thành công.{result}",
                orders = result
            });
        }

        //[HttpPost("generate-random-combo")]
        //[Authorize] // Đảm bảo người dùng đã đăng nhập
        //public async Task<IActionResult> GenerateRandomOrders([FromBody] GenerateRandomOrdersSingleRequest request)
        //{
        //    try
        //    {
        //        // Lấy UserId từ JWT claims
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (string.IsNullOrEmpty(userId))
        //            return Unauthorized("Không thể xác định người dùng.");

        //        var orders = await _orderService.CreateRandomOrdersWithSingleComboAsync(request, userId);
        //        return Ok(orders);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
        //    }
        //}
    }
}
