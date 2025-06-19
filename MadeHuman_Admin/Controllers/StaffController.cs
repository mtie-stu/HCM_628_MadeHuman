using MadeHuman_Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Admin.Controllers
{
    public class StaffController : Controller
    {
        private static List<ZoneProduct> _products = new List<ZoneProduct>
        {
            new ZoneProduct { Zone = "Inbound", WarehouseLocationsCode = "12345678", InventoryID = "INV001", SKUID = "SKU001", Quantity = 14 },
            new ZoneProduct { Zone = "Inbound", WarehouseLocationsCode = "12345679", InventoryID = "INV002", SKUID = "SKU002", Quantity = 7 },
            new ZoneProduct { Zone = "Inbound", WarehouseLocationsCode = "12345680", InventoryID = "INV003", SKUID = "SKU003", Quantity = 10 },
            new ZoneProduct { Zone = "Inbound", WarehouseLocationsCode = "12345681", InventoryID = "INV004", SKUID = "SKU004", Quantity = 20 },
            new ZoneProduct { Zone = "Inbound", WarehouseLocationsCode = "12345682", InventoryID = "INV005", SKUID = "SKU005", Quantity = 5 },
            new ZoneProduct { Zone = "Inbound", WarehouseLocationsCode = "12345683", InventoryID = "INV006", SKUID = "SKU006", Quantity = 3 },
            new ZoneProduct { Zone = "Outbound", WarehouseLocationsCode = "12345684", InventoryID = "INV007", SKUID = "SKU007", Quantity = 12 },
            new ZoneProduct { Zone = "Outbound", WarehouseLocationsCode = "12345685", InventoryID = "INV008", SKUID = "SKU008", Quantity = 8 },
            new ZoneProduct { Zone = "Outbound", WarehouseLocationsCode = "12345686", InventoryID = "INV009", SKUID = "SKU009", Quantity = 6 },
            new ZoneProduct { Zone = "Outbound", WarehouseLocationsCode = "12345687", InventoryID = "INV010", SKUID = "SKU010", Quantity = 9 },
        };
        // Dữ liệu tĩnh trong bộ nhớ
        private static List<PartTimeRoleHistory> _roleHistory = new()
        {
            new PartTimeRoleHistory { Date = new DateTime(2025, 6, 15), PartTimeId = "PT_001", Role = "Picker" },
            new PartTimeRoleHistory { Date = new DateTime(2025, 6, 16), PartTimeId = "PT_002", Role = "Checker" },
            new PartTimeRoleHistory { Date = new DateTime(2025, 6, 17), PartTimeId = "PT_003", Role = "Packer" }
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

            // Lọc theo zone
            if (!string.IsNullOrWhiteSpace(zone))
            {
                allProducts = allProducts
                    .Where(p => p.Zone.Equals(zone, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var keyword = searchTerm.Trim();
                allProducts = allProducts
                    .Where(p => p.SKUID.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Phân trang
            int pageSize = 5;
            int totalPages = (int)Math.Ceiling(allProducts.Count / (double)pageSize);
            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
            }

            var pagedProducts = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Chuẩn bị ViewModel
            var model = new ZoneManagementViewModel
            {
                Products = pagedProducts,
                CurrentPage = page,
                TotalPages = totalPages,
                SelectedZone = zone,
                SearchTerm = searchTerm,
                BaseUrl = Url.Action("ZoneManagement", "Staff", new { zone, searchTerm })
            };

            return View(model);
        }
        [HttpGet]                                                                             
        public IActionResult SetId()
        {
            return View(new SetPartTimeRoleViewModel
            {
                History = _roleHistory.OrderByDescending(x => x.Date).ToList()
            });
        }

        [HttpPost]
        public IActionResult SetId(SetPartTimeRoleViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PartTimeId) || string.IsNullOrWhiteSpace(model.Role))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                model.History = _roleHistory.OrderByDescending(x => x.Date).ToList();
                return View(model);
            }

            _roleHistory.Add(new PartTimeRoleHistory
            {
                Date = DateTime.Now,
                PartTimeId = model.PartTimeId.Trim(),
                Role = model.Role.Trim()
            });

            return RedirectToAction("SetId");
        }
        public IActionResult InboundTask()
        {
            var model = new InboundTaskViewModel
            {
                InboundReceiptsID = "RECEIPT123",
                InboundReceiptsItemsID = "ITEM456",
                ProductItemId = "PI-789",
                Quantity = "100",
                LocationStorage = "N/A",
                Zone = "",
                WarehouseLocationsCode = "",
                ProductImageUrl = "https://storage.googleapis.com/a1aa/image/bb8a5ce5-952e-4ad3-b719-429c2ebb7a23.jpg"
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult InboundTask(InboundTaskViewModel model)
        {
            // 🟡 Dữ liệu mẫu: chỉ in ra console/log
            Console.WriteLine("========= NHẬN DỮ LIỆU TỪ FORM =========");
            Console.WriteLine($"InboundReceiptsID: {model.InboundReceiptsID}");
            Console.WriteLine($"InboundReceiptsItemsID: {model.InboundReceiptsItemsID}");
            Console.WriteLine($"ProductItemId: {model.ProductItemId}");
            Console.WriteLine($"Quantity: {model.Quantity}");
            Console.WriteLine($"Zone: {model.Zone}");
            Console.WriteLine($"WarehouseLocationsCode: {model.WarehouseLocationsCode}");

            // 🟢 Trả lại view kèm thông báo
            ViewBag.Message = "✅ Tạo nhiệm vụ nhập kho thành công (dữ liệu tĩnh)";
            return View(model);
        }

    }
}
