using Microsoft.AspNetCore.Mvc;
using Madehuman_User.ViewModel.WareHouse;
using MadeHuman_User.ServicesTask.Services.WareHouseService;

namespace MadeHuman_User.Controllers
{
    public class WarehouseZoneController : Controller
    {
        private readonly IWarehouseZoneApiService _service;

        public WarehouseZoneController(IWarehouseZoneApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var zones = await _service.GetAllAsync();
            return View(zones);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var zone = await _service.GetByIdAsync(id);
            if (zone == null) return NotFound();
            return View(zone);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WareHouseZoneViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var created = await _service.CreateAsync(model);
            if (created != null) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Tạo khu vực kho thất bại");
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var zone = await _service.GetByIdAsync(id);
            if (zone == null) return NotFound();
            return View(zone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, WareHouseZoneViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _service.UpdateAsync(id, model);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Cập nhật khu vực kho thất bại");
            return View(model);
        }
    }
}
