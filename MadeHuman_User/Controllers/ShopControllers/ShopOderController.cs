using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers.ShopControllers
{
    public class ShopOderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
