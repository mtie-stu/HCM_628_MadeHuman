using Madehuman_Share.ViewModel;
using MadeHuman_User.Services.IServices;

namespace MadeHuman_User.ServicesTask.Services
{
    public class LoginService : ILoginService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<LoginResultDto?> LoginAsync(LoginModel model)
        {
            var client = _httpClientFactory.CreateClient("API");

            var response = await client.PostAsJsonAsync("api/Authentication/login", model);

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<LoginResultDto>();
        }
    }
}
