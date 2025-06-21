using MadeHuman_Server.Data;
using MadeHuman_Server.Service.Shop;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MadeHuman_Server.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ApplicationDbContext _context;
        public CategoryController(ICategoryService categoryService, ApplicationDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Không tìm thấy danh mục." });

            return Ok(category);
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _categoryService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.UpdateAsync(id, model);
            if (!result)
                return NotFound(new { message = "Không tìm thấy danh mục để cập nhật." });

            return Ok(new { message = "Cập nhật thành công." });
        }

       

        
    }
}
