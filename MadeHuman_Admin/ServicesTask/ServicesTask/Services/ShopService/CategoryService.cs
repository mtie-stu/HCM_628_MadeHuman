using Madehuman_User.ViewModel.Shop;

namespace MadeHuman_Admin.ServicesTask.Services.ShopService
{
    public interface ICategoryService
    {
        Task<List<CreateCategoryViewModel>> GetAllAsync();
        Task<CreateCategoryViewModel?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(CreateCategoryViewModel model);
        Task<bool> UpdateAsync(Guid id, CreateCategoryViewModel model);
    }
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _client;

        public CategoryService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("API");
        }
        public async Task<List<CreateCategoryViewModel>> GetAllAsync()
        {
            var response = await _client.GetAsync("api/Category");

            if (!response.IsSuccessStatusCode)
                return new List<CreateCategoryViewModel>();

            return await response.Content.ReadFromJsonAsync<List<CreateCategoryViewModel>>() ?? new();
        }

        public async Task<CreateCategoryViewModel?> GetByIdAsync(Guid id)
        {
            var response = await _client.GetAsync($"api/Category/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CreateCategoryViewModel>();
        }

        public async Task<bool> CreateAsync(CreateCategoryViewModel model)
        {
            var response = await _client.PostAsJsonAsync("api/Category", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid id, CreateCategoryViewModel model)
        {
            var response = await _client.PutAsJsonAsync($"api/Category/{id}", model);
            return response.IsSuccessStatusCode;
        }
    }
}
