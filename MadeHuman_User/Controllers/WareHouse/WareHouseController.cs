using Microsoft.AspNetCore.Mvc;
using Madehuman_User.ViewModel.WareHouse;
using MadeHuman_User.ServicesTask.Services.WareHouseService;

namespace MadeHuman_User.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IWarehouseApiService _warehouseService;

        public WarehouseController(IWarehouseApiService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // GET: /Warehouse
        public async Task<IActionResult> Index()
        {
            var list = await _warehouseService.GetAllAsync();
            return View(list);
        }

        // GET: /Warehouse/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var warehouse = await _warehouseService.GetByIdAsync(id);
            if (warehouse == null) return NotFound();
            return View(warehouse);
        }

        // GET: /Warehouse/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Warehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WareHouseViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var created = await _warehouseService.CreateAsync(model);
            if (created != null)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Tạo kho thất bại");
            return View(model);
        }

        // GET: /Warehouse/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var warehouse = await _warehouseService.GetByIdAsync(id);
            if (warehouse == null) return NotFound();
            return View(warehouse);
        }

        // POST: /Warehouse/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, WareHouseViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _warehouseService.UpdateAsync(id, model);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Cập nhật kho thất bại");
            return View(model);
        }
    }
}
