using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Service.Shop;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.AspNetCore.Mvc;

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

            return Ok(order);
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
            var result = await _orderService.CreateRandomOrdersWithSingleProductAsync(model);
            return Ok(new
            {
                message = $"Tạo {result.Count} đơn hàng thành công.{result}",
                orders = result
            });
        }

      /*  [HttpPost("generate-single-combo")]
        public async Task<IActionResult> GenerateRandomSingleCombo([FromBody] GenerateRandomOrdersSingleRequest model)
        {
            var result = await _orderService.CreateRandomOrdersWithSingleComboAsync(model);
            return Ok(new
            {
                message = $"Tạo {result.Count} đơn hàng thành công.",
                orders = result
            });
        }*/
    }
}
