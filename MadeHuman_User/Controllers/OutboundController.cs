using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    public class OutboundController : Controller
    {
        public IActionResult Picker()
        {
            return View();
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
