using Microsoft.AspNetCore.Mvc;
using Madehuman_Share.ViewModel;
using MadeHuman_User.ServicesTask.Services;


namespace MadeHuman_User.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService loginService)
        {
            _accountService = loginService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.LoginAsync(model);
            if (result == null)
            {
                TempData["Error"] = "Email hoặc mật khẩu không đúng!";
                return View(model);
            }

            await _accountService.StoreLoginCookiesAsync(result, HttpContext);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _accountService.RegisterAsync(model);

            if (!success)
            {
                TempData["Error"] = "Đăng ký thất bại! Email có thể đã tồn tại.";
                return View(model);
            }

            TempData["Success"] = "Đăng ký thành công! Hãy đăng nhập.";
            return RedirectToAction("Login");
        }
    }
}
