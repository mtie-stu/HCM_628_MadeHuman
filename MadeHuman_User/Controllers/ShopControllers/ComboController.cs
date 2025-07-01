using Madehuman_Share.ViewModel.Shop;
using MadeHuman_User.ServicesTask.Services.ShopService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers.ShopControllers
{
    public class ComboController : Controller
    {
        private readonly IComboService _comboService;

        public ComboController(IComboService comboService)
        {
            _comboService = comboService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var combos = await _comboService.GetAllAsync();
            return View(combos);
        }
        [HttpGet]
        public IActionResult CreateCombo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCombo(CreateComboViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _comboService.CreateComboAsync(model);

            if (result != null)
            {
                TempData["Success"] = "Tạo combo thành công!";
                return RedirectToAction("CreateCombo");
            }

            ModelState.AddModelError("", "Tạo combo thất bại.");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var combo = await _comboService.GetByIdAsync(id);
            if (combo == null)
            {
                return View("NotFound"); // hoặc return RedirectToAction("Index")
            }

            return View(combo);
        }
        [HttpGet]
        public IActionResult AddItems(Guid comboId)
        {
            var model = new AddComboItemsRequest
            {
                ComboId = comboId,
                Items = new List<ComboItemInputModel>
                {
                    new ComboItemInputModel()
                }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddItems(AddComboItemsRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _comboService.AddComboItemsAsync(model);
            if (success)
            {
                TempData["Success"] = "Thêm sản phẩm vào combo thành công!";
                return RedirectToAction("Detail", new { id = model.ComboId });
            }

            ModelState.AddModelError("", "Thêm sản phẩm thất bại.");
            return View(model);
        }
    }
}
