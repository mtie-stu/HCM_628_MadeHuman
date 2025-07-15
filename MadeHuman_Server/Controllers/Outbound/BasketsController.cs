using MadeHuman_Server.Services;
using Madehuman_Share;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MadeHuman_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly BasketService _basketService;

        public BasketsController(BasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _basketService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _basketService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BasketViewModel model)
        {
            var id = await _basketService.CreateAsync(model);
            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BasketViewModel model)
        {
            var updated = await _basketService.UpdateAsync(id, model);
            if (!updated) return NotFound();
            return Ok("Cập nhật thành công");
        }


    }
}
