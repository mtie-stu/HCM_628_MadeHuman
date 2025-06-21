using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Service.Shop;
using Madehuman_Share.ViewModel.Shop;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm." });

            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProduct_ProdcutSKU_ViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _productService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product updatedProduct)
        {
            var result = await _productService.UpdateAsync(id, updatedProduct);
            if (!result)
                return NotFound(new { message = "Không thể cập nhật sản phẩm." });

            return Ok(new { message = "Cập nhật thành công." });
        }

       
    }
}
