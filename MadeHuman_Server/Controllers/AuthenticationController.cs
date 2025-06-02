using MadeHuman_Server.Model;
using MadeHuman_Server.Service;
using MadeHuman_Server.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MadeHuman_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        public AuthenticationController(ITokenService tokenService,
                                        SignInManager<AppUser> signInManager,
                                        UserManager<AppUser> userManager)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message =
                    "Dữ liệu không hợp lệ",
                    errors =
                    ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                });
            }

            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await
                _userManager.FindByEmailAsync(model.Email!);
            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message =
                    "Email đã được sử dụng!"
                });
            }

            var user = new AppUser
            {
                UserName = model.Name,
                NormalizedUserName = model.Name!.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email!.ToUpper(),
                EmailConfirmed = true
            };

            var result = await
                _userManager.CreateAsync(user, model.Password!);

            if (!result.Succeeded)
            {
                return BadRequest(
                    new
                    {
                        message = "Đăng ký thất bại!",
                        errors = result.Errors
                    });
            }

            await _userManager.AddToRoleAsync(user, "User");
            return Ok(new
            {
                message = "Đăng ký thành công!",
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }
            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password!, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });
            }
            var token = await _tokenService.GenerateJwtToken(user);
            return Ok(new
            {
                token,
                userId = user.Id,
                email = user.Email
            });
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Đăng xuất thành công!" });
        }
    }
}
