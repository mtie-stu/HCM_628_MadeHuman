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
            return View();
        }
        public IActionResult CheckerTaskSingle()
        {
            return View();
        }
        public IActionResult Packer()
        {
            return View();
        }
        public IActionResult Dispatch()
        {
            return View();
        }
    }
}
