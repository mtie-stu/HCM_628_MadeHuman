using Madehuman_Share.ViewModel.WareHouse;
using MadeHuman_User.ServicesTask.Services.Warehouse;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    public class WareHouseLocationController : Controller
    {
        private readonly IWarehouseLocationServices _svc;

        public WareHouseLocationController(IWarehouseLocationServices svc)
        {
            _svc = svc;
        }
        [HttpGet("/warehouse/all")]
        public async Task<IActionResult> All(int page = 1, string searchTerm = "", CancellationToken ct = default)
        {
            const int pageSize = 6; // giống ví dụ bạn dùng

            // Lấy toàn bộ (view model)
            var all = await _svc.GetAllAsync(ct); // trả về List<WarehouseLocationViewModel>

            // Lọc theo từ khoá: NameLocation / ZoneId / Id
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var q = searchTerm.Trim();
                all = all.Where(x =>
                        (!string.IsNullOrEmpty(x.NameLocation) && x.NameLocation.Contains(q, StringComparison.OrdinalIgnoreCase))
                     || x.ZoneId.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)
                     || x.Id.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Tính trang
            var totalItems = all.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            var currentPage = Math.Min(Math.Max(1, page), totalPages);

            var data = all
                .OrderBy(x => x.NameLocation) // sắp xếp ổn định
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBag cho phân trang + filter
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

            return View(data); // View: Views/Warehouse/All.cshtml
        }


        [HttpPost("/warehouse/create")]
        public async Task<IActionResult> Create([FromForm] WarehouseLocationViewModel vm, CancellationToken ct)
        {
            var created = await _svc.CreateAsync(vm, ct);
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }

        [HttpGet("/warehouse/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var found = await _svc.GetByIdAsync(id, ct);
            if (found == null) return NotFound();
            return View(found);
        }

        [HttpPost("/warehouse/edit/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] WarehouseLocationViewModel vm, CancellationToken ct)
        {
            var updated = await _svc.UpdateAsync(id, vm, ct);
            return RedirectToAction(nameof(Details), new { id = updated.Id });
        }

        [HttpPost("/warehouse/generate")]
        public async Task<IActionResult> Generate([FromForm] GenerateWHLocationRequest req, CancellationToken ct)
        {
            var created = await _svc.GenerateLocationsAsync(req, ct);
            TempData["GeneratedCount"] = created.Count;
            return RedirectToAction(nameof(All));
        }
    }
}
