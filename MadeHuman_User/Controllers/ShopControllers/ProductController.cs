using Madehuman_Share.ViewModel.Shop;
using MadeHuman_User.ServicesTask.Services.ShopService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers.ShopControllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }


        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _productService.GetProductDetailAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProduct_ProdcutSKU_ViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _productService.CreateAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Tạo sản phẩm thất bại.");
                return View(model);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _productService.GetProductDetailAsync(id);
            if (product == null) return NotFound();
            return View(product); // truyền sang View để hiển thị sẵn
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, CreateProduct_ProdcutSKU_ViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _productService.UpdateAsync(id, model);
            if (!success)
            {
                TempData["Error"] = "Cập nhật thất bại!";
                return View(model);
            }

            return RedirectToAction("Index");
        }

    }
}
