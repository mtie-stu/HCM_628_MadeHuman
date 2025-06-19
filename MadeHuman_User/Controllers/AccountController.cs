using Microsoft.AspNetCore.Mvc;
using MadeHuman_User.Models;

namespace MadeHuman_User.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            // Bỏ layout
            ViewData["Title"] = "Login";
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.IsValidPasswordFormat)
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters and include 1 uppercase letter, 1 number, and 1 special character.");
                return View(model);
            }

            // Dữ liệu tĩnh
            var staticUsers = new List<LoginViewModel>
            {
                new LoginViewModel { EmailOrID = "admin01", Password = "Admin@123" },
                new LoginViewModel { EmailOrID = "admin@example.com", Password = "Admin@123" },
                new LoginViewModel { EmailOrID = "user01", Password = "User@123" },
                new LoginViewModel { EmailOrID = "user@example.com", Password = "User@123" }
            };

            var user = staticUsers.FirstOrDefault(u =>
                u.EmailOrID.Equals(model.EmailOrID, StringComparison.OrdinalIgnoreCase) &&
                u.Password == model.Password
            );

            if (user != null)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login credentials");
            return View(model);
        }
    }
}
