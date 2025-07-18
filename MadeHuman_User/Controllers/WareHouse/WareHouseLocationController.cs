using Microsoft.AspNetCore.Mvc;
using MadeHuman_User.ServicesTask.Services.WareHouseService;

using Microsoft.AspNetCore.Mvc.Rendering;
using Madehuman_Share.ViewModel.WareHouse;

namespace MadeHuman_User.Controllers
{
    public class WarehouseLocationController : Controller
    {
        private readonly IWarehouseLocationApiService _service;
        private readonly IWarehouseZoneApiService _zoneservice;

        public WarehouseLocationController(IWarehouseLocationApiService service, IWarehouseZoneApiService zoneservice)
        {
            _service = service;
            _zoneservice = zoneservice;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var location = await _service.GetByIdAsync(id);
            if (location == null) return NotFound();
            return View(location);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseLocationViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var created = await _service.CreateAsync(model);
            if (created != null) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Tạo vị trí kho thất bại");
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var location = await _service.GetByIdAsync(id);
            if (location == null) return NotFound();
            return View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, WarehouseLocationViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _service.UpdateAsync(id, model);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Cập nhật vị trí kho thất bại");
            return View(model);
        }

     
        // POST: /WarehouseLocation/Generate
        [HttpGet]
        public async Task<IActionResult> Generate()
        {
            ViewBag.ZoneList = new SelectList(await _zoneservice.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(GenerateWHLocationRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ZoneList = new SelectList(await _service.GetAllAsync(), "Id", "Name");
                return View(request);
            }

            var result = await _service.GenerateLocationsAsync(request);



            return View("~/Views/WarehouseLocation/GeneratedList.cshtml", result);
        }

    }
}
