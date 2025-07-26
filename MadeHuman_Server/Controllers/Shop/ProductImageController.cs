using MadeHuman_Server.Service.Shop;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Shop
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _productImageService;

        public ProductImageController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        [HttpPost("get-images")]
        public async Task<IActionResult> GetImages([FromBody] List<Guid> productSKUIds)
        {
            var result = await _productImageService.GetImageUrlsByProductSKUIdsAsync(productSKUIds);
            return Ok(result);
        }
    }
}
