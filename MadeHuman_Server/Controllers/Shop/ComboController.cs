using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Service.Shop;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MadeHuman_Server.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComboController : ControllerBase
    {
        private readonly IComboService _comboService;

        public ComboController(IComboService comboService)
        {
            _comboService = comboService;
        }

        // GET: api/combo
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var combos = await _comboService.GetAllAsync();
            return Ok(combos);
        }

        // GET: api/combo/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var combo = await _comboService.GetByIdAsync(id);
            if (combo == null)
                return NotFound(new { message = "Không tìm thấy combo." });

            return Ok(combo);
        }

        //POST: api/combo

        /// <summary>
        /// Tạo combo cơ bản (chưa có ComboItems)
        /// </summary>
        [HttpPost("create-base")]
        [ProducesResponseType(typeof(Combo), 200)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateBase([FromForm] CreateComboViewModel model)
        {
            var combo = await _comboService.CreateComboAsync(model);
            return Ok(combo);
        }

        /// <summary>
        /// Thêm danh sách sản phẩm cho combo
        /// </summary>
        [HttpPost("add-items")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddItems([FromBody] AddComboItemsRequest model)
        {
            if (model.Items == null || model.Items.Count == 0)
                return BadRequest("Danh sách sản phẩm không được trống.");

            await _comboService.AddComboItemsAsync(model);
            return Ok(new { message = "Thêm ComboItems thành công." });
        }


        // PUT: api/combo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateComboWithItemsViewModel combo)
        {
            if (id != combo.ComboId)
                return BadRequest(new { message = "ID không khớp." });

            var result = await _comboService.UpdateAsync(id, combo);
            if (!result)
                return NotFound(new { message = "Không tìm thấy combo để cập nhật." });

            return Ok(new { message = "Cập nhật thành công." });
        }


    }
}
