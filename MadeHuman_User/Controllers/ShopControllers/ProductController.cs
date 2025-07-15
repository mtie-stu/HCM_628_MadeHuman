using Madehuman_Share.ViewModel.Shop;
using MadeHuman_User.ServicesTask.Services.ShopService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;

namespace MadeHuman_User.Controllers.ShopControllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
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
            var detail = await _productService.GetProductDetailAsync(id);
            if (detail == null) return NotFound();

            var editModel = new CreateProduct_ProdcutSKU_ViewModel
            {
                ProductId = detail.ProductId,
                Name = detail.Name,
                Description = detail.Description,
                Price = detail.Price,
                SKU = detail.SKU,
                //QuantityInStock = detail.QuantityInStock,
                CategoryId = detail.CategoryId ?? Guid.Empty,
                CategoryName = detail.CategoryName
            };
            var categories = await _categoryService.GetAllAsync() ?? new();

            Guid? selectedCategoryId = categories.Any(c => c.CategoryId == editModel.CategoryId)
                ? editModel.CategoryId
                : (Guid?)null;

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", selectedCategoryId);

            return View(editModel);
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
