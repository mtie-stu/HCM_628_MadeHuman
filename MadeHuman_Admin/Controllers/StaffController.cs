using MadeHuman_Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Admin.Controllers
{
    public class StaffController : Controller
    {
        // Dữ liệu tĩnh
        private static List<ZoneProduct> _products = new List<ZoneProduct>
        {
            new ZoneProduct { Zone = "Inbound", WarehouseLocationsCode = "12345678", InventoryID = "INV001", SKUID = "ABC", Quantity = 14 },
            new ZoneProduct { Zone = "Inbound", WarehouseLocationsCode = "12345679", InventoryID = "INV002", SKUID = "DEF", Quantity = 7 },
            new ZoneProduct { Zone = "Outbound", WarehouseLocationsCode = "12345680", InventoryID = "INV003", SKUID = "XYZ", Quantity = 10 },
            new ZoneProduct { Zone = "Outbound", WarehouseLocationsCode = "12345681", InventoryID = "INV004", SKUID = "LMN", Quantity = 20 },
            // Thêm dữ liệu test thoải mái
        };
        private List<ZoneProduct> GetStaticProducts()
        {
            return _products;
        }

        [HttpGet]
        public IActionResult ZoneManagement(int page = 1, string zone = "", string searchTerm = "")
        {
            return ProcessZoneManagement(page, zone, searchTerm);
        }

        [HttpPost]
        public IActionResult ZoneManagement(string zone, string searchTerm)
        {
            // Khi filter -> reset về trang 1
            return RedirectToAction(nameof(ZoneManagement), new { page = 1, zone, searchTerm });
        }

        public IActionResult ProcessZoneManagement(int page, string zone, string searchTerm)
        {
            var allProducts = GetStaticProducts();

            if (!string.IsNullOrWhiteSpace(zone))
                allProducts = allProducts.Where(x => x.Zone == zone).ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                allProducts = allProducts.Where(x => x.SKUID.Contains(searchTerm)).ToList();

            int pageSize = 5;
            int totalPages = (int)Math.Ceiling(allProducts.Count / (double)pageSize);

            var pagedProducts = allProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SelectedZone = zone;
            ViewBag.SearchTerm = searchTerm;

            // BaseUrl giữ tham số filter
            string baseUrl = Url.Action("ZoneManagement", "Staff", new { zone, searchTerm });
            ViewBag.BaseUrl = baseUrl;

            return View(pagedProducts);
        }

    }
}
