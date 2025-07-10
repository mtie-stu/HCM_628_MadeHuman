using MadeHuman_Server.Service.WareHouse;
using Madehuman_Share.ViewModel.WareHouse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MadeHuman_Server.Controllers.WareHouse
{
    [ApiController]
    [Route("api/[controller]")]
    public class WareHouseLocationController : ControllerBase
    {
        private readonly IWarehouseLocationService _service;

        public WareHouseLocationController(IWarehouseLocationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(WarehouseLocationViewModel warehouse)
        {
            var result = await _service.CreateAsync(warehouse);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, WarehouseLocationViewModel warehouse)
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

        [HttpPost("generate-locations")]
        public async Task<IActionResult> GenerateLocations([FromBody] GenerateWHLocationRequest request)
        {
            var result = await _service.GenerateLocationsAsync
            (
                request.ZoneId,
                request.StartLetter,
                request.EndLetter,
                request.StartNumber,
                request.EndNumber,
                request.StartSub,
                request.EndSub,
                request.Quantity
            );

            return Ok(result);
        }




    }
}
