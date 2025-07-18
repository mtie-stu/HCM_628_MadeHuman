using System.Net.Http.Json;
using Madehuman_Share.ViewModel.WareHouse;
using Madehuman_User.ViewModel.WareHouse;

namespace MadeHuman_User.ServicesTask.Services.WareHouseService
{
    public interface IWarehouseLocationApiService
    {
        Task<List<WarehouseLocationViewModel>?> GetAllAsync();
        Task<WarehouseLocationViewModel?> GetByIdAsync(Guid id);
        Task<WarehouseLocationViewModel?> CreateAsync(WarehouseLocationViewModel model);
        Task<bool> UpdateAsync(Guid id, WarehouseLocationViewModel model);
        Task<List<WarehouseLocationViewModel>?> GenerateLocationsAsync(GenerateWHLocationRequest request);
    }
    public class WarehouseLocationApiService : IWarehouseLocationApiService
    {
        private readonly HttpClient _httpClient;

        public WarehouseLocationApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("API");
        }

        public async Task<List<WarehouseLocationViewModel>?> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<WarehouseLocationViewModel>>("api/WareHouseLocation");
        }

        public async Task<WarehouseLocationViewModel?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<WarehouseLocationViewModel>($"api/WareHouseLocation/{id}");
        }

        public async Task<WarehouseLocationViewModel?> CreateAsync(WarehouseLocationViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/WareHouseLocation", model);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<WarehouseLocationViewModel>()
                : null;
        }

        public async Task<bool> UpdateAsync(Guid id, WarehouseLocationViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/WareHouseLocation/{id}", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<WarehouseLocationViewModel>?> GenerateLocationsAsync(GenerateWHLocationRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/WareHouseLocation/generate-locations", request);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<WarehouseLocationViewModel>>()
                : null;
        }
    }
}
