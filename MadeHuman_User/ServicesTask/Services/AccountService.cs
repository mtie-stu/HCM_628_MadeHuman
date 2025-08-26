using Madehuman_User.ViewModel;
//using MadeHuman_Share.ViewModel;
using System.Net.Http;

namespace MadeHuman_User.ServicesTask.Services
{
    public interface IAccountService
    {
        Task<LoginResultDto?> LoginAsync(LoginModel model);
        Task<bool> RegisterAsync(RegisterModel model);
        Task StoreLoginCookiesAsync(LoginResultDto result, HttpContext httpContext);
    }
    public class AccountService : IAccountService
    { 

        private readonly HttpClient _client;

        public AccountService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("API");
        }

        public async Task<LoginResultDto?> LoginAsync(LoginModel model)
        {

            var response = await _client.PostAsJsonAsync("api/Authentication/login", model);

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<LoginResultDto>();
        }
        public async Task<bool> RegisterAsync(RegisterModel model)
        {

            var response = await _client.PostAsJsonAsync("api/Authentication/register", model);

            return response.IsSuccessStatusCode;
        }

        public Task StoreLoginCookiesAsync(LoginResultDto result, HttpContext httpContext)
        {
            var commonOptions = new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            };

            // Token: HttpOnly để tránh bị JS đọc
            var tokenOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = commonOptions.Expires
            };

            httpContext.Response.Cookies.Append("JWTToken", result.Token, tokenOptions);
            httpContext.Response.Cookies.Append("UserId", result.UserId, commonOptions);
            httpContext.Response.Cookies.Append("EmailOrId", result.Email, commonOptions);
            httpContext.Response.Cookies.Append("UserRole", result.Role, commonOptions); // ⬅️ Thêm dòng này
            return Task.CompletedTask;
        }


    }
}
