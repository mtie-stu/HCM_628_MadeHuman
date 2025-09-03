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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 8, string? q = "")
        {
            if (page < 1) page = 1;
            pageSize = (pageSize is < 1 or > 100) ? 8 : pageSize;

            // Lấy toàn bộ (fallback về list rỗng)
            var all = await _productService.GetAllAsync() ?? new List<ProductListItemViewModel>();

            // ----- Filter theo từ khoá -----
            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.Trim().ToLowerInvariant();
                all = all.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(ql)) ||
                    (!string.IsNullOrEmpty(p.SKU) && p.SKU.ToLower().Contains(ql)) ||
                    (!string.IsNullOrEmpty(p.CategoryName) && p.CategoryName.ToLower().Contains(ql))
                ).ToList();
            }

            // ----- Phân trang -----
            var totalItems = all.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            if (page > totalPages) page = totalPages;

            var data = all
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Thông tin phục vụ View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.Q = q ?? "";
            ViewBag.TotalItems = totalItems;
            ViewBag.FromItem = totalItems == 0 ? 0 : (page - 1) * pageSize + 1;
            ViewBag.ToItem = Math.Min(page * pageSize, totalItems);

            return View(data);
        }



        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _productService.GetProductDetailAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAsync(); // trả về List<CreateCategoryViewModel>
            ViewBag.Categories = categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View(new CreateProduct_ProdcutSKU_ViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProduct_ProdcutSKU_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(model);
            }

            var success = await _productService.CreateAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Tạo sản phẩm thất bại.");

                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    })
                    .ToList();

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
