using Microsoft.AspNetCore.Mvc;
using Madehuman_Share.ViewModel;
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
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _loginService.LoginAsync(model);
            if (result == null)
            {
                TempData["Error"] = "Email hoặc mật khẩu không đúng!";
                return View(model);
            }

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("UserId", result.UserId);
            HttpContext.Session.SetString("Email", result.Email);

            return RedirectToAction("Index", "Home");
        }
    }
}
