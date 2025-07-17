using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace MadeHuman_Admin.JWT
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["JWTToken"]; // ⬅️ Lấy từ cookie thay vì Header
            if (!string.IsNullOrEmpty(token))
            {
                var principal = ValidateJwtToken(token);
                if (principal != null)
                {
                    // Đăng nhập vào hệ thống với Claims từ JWT
                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                }
            }

            await _next(context);
        }

        private ClaimsPrincipal? ValidateJwtToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]!);

                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["AuthSettings:Issuer"],
                    ValidAudience = _configuration["AuthSettings:Audience"],
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParams, out _);
                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ JWT Middleware validation failed: {ex.Message}");
                return null;
            }
        }
    }
}

