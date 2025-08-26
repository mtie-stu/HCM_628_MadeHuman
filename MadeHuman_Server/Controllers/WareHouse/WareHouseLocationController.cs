using MadeHuman_Server.Model.WareHouse;
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
                //
                request.ZoneId,
                request.StartLetter,
                request.EndLetter,
                request.StartNumber,
                request.EndNumber,
                request.StartSub,//1
                request.EndSub,//2
                request.Quantity//số lượng location
            );

            return Ok(result);
        }

        [HttpGet("location/{id}")]
        public async Task<IActionResult> GetLocationInfo(Guid id)
        {
            var result = await _service.GetLocationInfoAsync(id);

            if (result == null)
                return NotFound(new { message = "❌ Không tìm thấy vị trí kho." });

            return Ok(result);
        }

        /// <summary>
        /// GET /api/WareHouseLocation/options?zoneId={guid}&status=Stored
        /// Nhận status dạng enum (Empty/Stored/...)
        /// </summary>
        [HttpGet("options")]
        public async Task<IActionResult> GetOptions([FromQuery] Guid zoneId, [FromQuery] StatusWareHouse status)
        {
            if (zoneId == Guid.Empty)
                return BadRequest(new { message = "zoneId is required." });

            var list = await _service.GetOptionsAsync(zoneId, status);
            return Ok(list);
        }

        /// <summary>
        /// GET /api/WareHouseLocation/options/by-string?zoneId={guid}&status=Stored
        /// Nhận status dạng chuỗi, tự parse sang enum và báo lỗi nếu sai.
        /// </summary>
        [HttpGet("options/by-string")]
        public async Task<IActionResult> GetOptionsByString([FromQuery] Guid zoneId, [FromQuery] string status)
        {
            if (zoneId == Guid.Empty)
                return BadRequest(new { message = "zoneId is required." });

            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { message = "status is required." });

            if (!Enum.TryParse<StatusWareHouse>(status, ignoreCase: true, out var enumStatus))
                return BadRequest(new { message = $"Invalid status '{status}'. Accepted: {string.Join(", ", Enum.GetNames(typeof(StatusWareHouse)))}" });

            var list = await _service.GetOptionsAsync(zoneId, enumStatus);
            return Ok(list);
        }
    }
}
