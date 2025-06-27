using Madehuman_Share.ViewModel;
using MadeHuman_User.Services.IServices;

namespace MadeHuman_User.ServicesTask.Services
{
    public class LoginService : ILoginService
    { 

        private readonly HttpClient _client;

        public LoginService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("API");
        }

        public async Task<LoginResultDto?> LoginAsync(LoginModel model)
        {

            var response = await _client.PostAsJsonAsync("api/Authentication/login", model);

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<LoginResultDto>();
        }
    }
}
