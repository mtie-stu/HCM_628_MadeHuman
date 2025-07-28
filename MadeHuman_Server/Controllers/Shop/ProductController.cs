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
        private readonly IProductLookupService _productLookupService;   
        public ProductController(IProductService productService,IProductLookupService productLookupService)
        {
            _productService = productService;
            _productLookupService = productLookupService;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            var result = products.Select(product => new ProductListItemViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                SKU = product.ProductSKU?.SKU,
                CategoryName = product.Category?.Name
            });
            return Ok(result);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm." });

            var result = new ProductDetailViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.ProductSKU?.SKU,
                CategoryId = product.CategoryId, // ✅ thêm dòng này nếu bạn cần để sửa trên FE
                CategoryName = product.Category?.Name,

                ProductItems = product.ProductItems?.Select(item => new ProductItemDto
                {
                    SKU = item.SKU,
                }).ToList() ?? new List<ProductItemDto>()
            };


            return Ok(result);
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
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateProduct_ProdcutSKU_ViewModel updated)
        {
            var result = await _productService.UpdateAsync(id, updated);
            if (!result)
                return NotFound(new { message = "Không thể cập nhật sản phẩm." });

            return Ok(new { message = "Cập nhật thành công." });
        }
        [HttpGet("sku/{id}")]
        public async Task<IActionResult> GetSKUInfo(Guid id)
        {
            var result = await _productLookupService.GetSKUInfoAsync(id);
            if (result == null)
                return NotFound(new { message = "❌ Không tìm thấy ProductSKU." });

            return Ok(result);
        }



    }
}
