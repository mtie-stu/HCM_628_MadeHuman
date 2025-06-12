using MadeHuman_User.Models;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    public class InboundController : Controller
    {
        private static readonly List<(string ExportCode, ProductExportViewModel Product)> exportData = new()
        {
            ("EXP001", new ProductExportViewModel
            {
                ProductItemId = "PI-001",
                ProductName = "Sản phẩm A",
                ImageUrl = "https://storage.googleapis.com/a1aa/image/5d961ab1-365e-4f05-5bc4-3d928fc083a3.jpg",
                LocationStorage = "Kho A1",
                Quantity = 5
            }),
            ("EXP002", new ProductExportViewModel
            {
                ProductItemId = "PI-002",
                ProductName = "Sản phẩm B",
                ImageUrl = "https://storage.googleapis.com/a1aa/image/5d961ab1-365e-4f05-5bc4-3d928fc083a3.jpg",
                LocationStorage = "Kho B2",
                Quantity = 8
            })
        };
        private static readonly List<ProductImportViewModel> taskProducts = new()
        {
            new ProductImportViewModel
            {
                TaskCode = "TASK001",
                ProductItemId = "SP001",
                ProductName = "Sản phẩm A",
                Quantity = 100,
                LocationStorage = "Kệ A1",
                ImageUrl = "https://via.placeholder.com/300x180.png?text=SanPham+A"
            },
            new ProductImportViewModel
            {
                TaskCode = "TASK001",
                ProductItemId = "SP002",
                ProductName = "Sản phẩm B",
                Quantity = 50,
                LocationStorage = "Kệ A2",
                ImageUrl = "https://via.placeholder.com/300x180.png?text=SanPham+B"
            }
        };

        // Trang nhập mã để quét task export
        [HttpGet]
        public IActionResult Export()
        {
            return View(new ExportTaskViewModel());
        }

        [HttpPost]
        public IActionResult Export(ExportTaskViewModel model)
        {
            // Lấy danh sách sản phẩm thuộc task export (theo mã export)
            var taskProducts = exportData
                .Where(p => p.ExportCode == model.ExportCode)
                .ToList(); // Không cần Select vì đã là ProductExportViewModel

            if (!taskProducts.Any())
            {
                ModelState.AddModelError("", "Không tìm thấy mã Export.");
                return View(model);
            }

            // Gán danh sách sản phẩm vào model để hiển thị lại
            model.Products = taskProducts.Select(p => p.Product).ToList();


            // Tìm sản phẩm phù hợp theo mã sản phẩm và vị trí quét
            var matchedProduct = taskProducts
             .Select(p => p.Product)
             .FirstOrDefault(p =>
                 p.ProductItemId == model.ScannedProductCode &&
                 p.LocationStorage == model.ScannedLocationCode);


            if (matchedProduct == null)
            {
                ModelState.AddModelError("", "Mã vị trí hoặc mã sản phẩm không khớp với nhiệm vụ xuất kho.");
                return View(model);
            }

            // Kiểm tra số lượng
            if (model.ScannedQuantity <= 0 || model.ScannedQuantity > matchedProduct.Quantity)
            {
                ModelState.AddModelError("", $"Số lượng không hợp lệ. Số lượng cần xuất còn lại: {matchedProduct.Quantity}");
                return View(model);
            }

            // Trừ số lượng giả lập (vì đang dùng dữ liệu tĩnh)
            matchedProduct.Quantity -= model.ScannedQuantity;

            ViewBag.Success = "Xuất kho thành công!";
            return View(model);
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View(new ImportTaskViewModel());
        }

        [HttpPost]
        public IActionResult Import(ImportTaskViewModel model)
        {
            // Tìm danh sách sản phẩm thuộc nhiệm vụ được quét
            model.Products = taskProducts
                .Where(p => p.TaskCode == model.TaskCode)
                .ToList();

            if (!model.Products.Any())
            {
                ModelState.AddModelError("", "Không tìm thấy nhiệm vụ nhập hàng.");
                return View(model);
            }

            // Nếu người dùng quét mã sản phẩm
            if (!string.IsNullOrWhiteSpace(model.ScannedProductCode))
            {
                var matched = model.Products
                    .FirstOrDefault(p => p.ProductItemId == model.ScannedProductCode);

                if (matched == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy sản phẩm trong nhiệm vụ.");
                    return View(model);
                }

                model.SelectedProduct = matched;

                // Nếu có nhập số lượng
                if (model.ScannedQuantity.HasValue)
                {
                    if (model.ScannedQuantity <= 0 || model.ScannedQuantity > matched.Quantity)
                    {
                        ModelState.AddModelError("", $"Số lượng không hợp lệ. Tối đa: {matched.Quantity}");
                        return View(model);
                    }

                    // Trừ số lượng (giả lập)
                    matched.Quantity -= model.ScannedQuantity.Value;

                    ViewBag.Success = $"✅ Nhập thành công {model.ScannedQuantity.Value} sản phẩm: {matched.ProductName}";

                    // Reset form cho lượt tiếp theo
                    model.ScannedProductCode = "";
                    model.ScannedQuantity = null;
                    model.SelectedProduct = null;
                }
            }

            return View(model);
        }

    }
}
