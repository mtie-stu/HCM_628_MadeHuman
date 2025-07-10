using MadeHuman_User.Models;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    public class OutboundController : Controller
    {
        public IActionResult Picker()
        {
            var model = new PickerViewModel
            {
                TaskId = 3122,
                ProductItem = "SP001",
                LocationStorage = "A1-02",
                Baskets = "5",
                Quantity = 20,
                ProductName = "Sản phẩm mẫu",
                ImageUrl = "/images/sample-product.png" // Đảm bảo ảnh tồn tại trong wwwroot/images/
            };
            return View(model);
        }

        public IActionResult CheckerTaskMix()
        {
            var viewModel = new CheckerTaskMixViewModel
            {
                TaskId = "TASK-MIX-001",
                ProductItem = "Combo Item A",
                LocationStorage = "Warehouse A - Shelf 3",
                Baskets = "Basket X, Basket Y",
                Quantity = 100,
                ImageUrl = "/images/product/comboA.jpg",
                ProductTasks = new List<CheckerTaskItem>
        {
            new CheckerTaskItem
            {
                Status = "Pending",
                SmallTaskID = "ST-001",
                ProductItemID = "PI-001",
                ProductName = "Item 1",
                Quantity = 30,
                LocationStorage = "Warehouse A - Shelf 1"
            },
            new CheckerTaskItem
            {
                Status = "Completed",
                SmallTaskID = "ST-002",
                ProductItemID = "PI-002",
                ProductName = "Item 2",
                Quantity = 70,
                LocationStorage = "Warehouse A - Shelf 2"
            }
        }
            };

            return View(viewModel);
        }
        public IActionResult CheckerTaskSingle()
        {
            var viewModel = new CheckerTaskSingleViewModel
            {
                TaskId = "TASK-SINGLE-001",
                ProductItemId = "PI-123",
                ProductName = "Product A",
                Quantity = 100,
                LocationStorage = "Warehouse A",
                ImageUrl = "/images/product/productA.jpg",
                InputProductCode = "",
                InputQuantity = 0
            };

            return View(viewModel);
            }
        public IActionResult Packer()
        {
            var model = new PackerViewModel
            {
                BasketId = "BASKET123",
                CurrentPage = 1,
                TotalPages = 1,
                TaskItems = new List<PackerViewModel.TaskItem>
            {
                new PackerViewModel.TaskItem
                {
                    TaskId = "ST001",
                    ProductItemId = "PI001",
                    ProductName = "Sản phẩm A",
                    Quantity = 3,
                    LocationStorage = "Kho A1",
                    Status = "Done"
                }
            }
            };

            return View("Packer", model); // hoặc chỉ View(model) nếu file trùng tên action
        }

        public IActionResult Dispatch()
        {
            return View();
        }
    }
}
