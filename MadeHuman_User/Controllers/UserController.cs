using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    public class UserController : Controller
    {
        public IActionResult CheckInOut()
        {
            return View();
        }
        public IActionResult Reports()
        {
            return View();
        }
    }
}
