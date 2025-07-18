using Madehuman_User.ViewModel.Shop;
using MadeHuman_Admin.ServicesTask.Services.ShopService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Admin.Controllers.ShopControllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }
        public async Task<IActionResult> Index()
        {
            var list = await _categoryService.GetAllAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _categoryService.CreateAsync(model);
            if (!result)
            {
                TempData["Error"] = "Thêm danh mục thất bại.";
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _categoryService.UpdateAsync(id, model);
            if (!result)
            {
                TempData["Error"] = "Cập nhật thất bại.";
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
