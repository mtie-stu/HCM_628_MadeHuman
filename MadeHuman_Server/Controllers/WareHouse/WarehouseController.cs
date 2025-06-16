using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Madehuman_Share.ViewModel.WareHouse;
using MadeHuman_Server.Model.WareHouse;
using MadeHuman_Server.Service.WareHouse;


namespace MadeHuman_Server.Controllers.WareHouse
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _service;

        public WarehousesController(IWarehouseService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(WareHouseViewModel warehouse)
        {
            var result = await _service.CreateAsync(warehouse);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, WareHouseViewModel warehouse)
        {
            var result = await _service.UpdateAsync(id, warehouse);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync(); // phải là Task<...>
            return Ok(result);
        }

    }

}
