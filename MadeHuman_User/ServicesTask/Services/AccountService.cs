using Madehuman_Share.ViewModel;

using System.Net.Http;

namespace MadeHuman_User.ServicesTask.Services
{
    public interface IAccountService
    {
        Task<LoginResultDto?> LoginAsync(LoginModel model);
        Task<bool> RegisterAsync(RegisterModel model);
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
    }
}
