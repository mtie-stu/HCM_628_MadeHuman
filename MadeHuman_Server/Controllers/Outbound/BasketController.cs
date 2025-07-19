using MadeHuman_Server.Service.Outbound;
using Madehuman_Share.ViewModel.Basket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Outbound
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        // POST: api/basket/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateBasketViewModel model)
        {
            var result = await _basketService.AddAsync(model);
            return Ok(result);
        }

        // POST: api/basket/create-many
        [HttpPost("create-many")]
        public async Task<IActionResult> CreateMany([FromBody] CreateBasketRangeViewModel model)
        {
            if (model.Quantity <= 0)
                return BadRequest("Số lượng phải lớn hơn 0.");

            var result = await _basketService.AddRangeAsync(model);
            return Ok(result);
        }

        // GET: api/basket/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _basketService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // GET: api/basket
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _basketService.GetAllAsync();
            return Ok(result);
        }

        // PUT: api/basket/update-outbound-task
        [HttpPut("update-outbound-task")]
        public async Task<IActionResult> UpdateOutBoundTask([FromBody] UpdateBasketOutboundTaskViewModel model)
        {
            var success = await _basketService.UpdateOutBoundTaskIdAsync(model);
            if (!success)
                return NotFound();

            return Ok(new { message = "Cập nhật OutBoundTaskId thành công." });
        }
    }
}
