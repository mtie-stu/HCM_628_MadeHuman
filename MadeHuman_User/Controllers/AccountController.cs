using Microsoft.AspNetCore.Mvc;
using MadeHuman_Share.ViewModel;
using MadeHuman_User.Services;
using MadeHuman_User.Services.IServices;

namespace MadeHuman_User.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService _loginService;

        public AccountController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_loginService.ValidateUser(model.EmailOrID, model.Password))
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ");
            return View(model);
        }
    }
}
