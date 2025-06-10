using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    public class InboundController : Controller
    {
        public IActionResult Import()
        {
            return View();
        }
        public IActionResult Export()
        {
            return View();
        }
    }
}
