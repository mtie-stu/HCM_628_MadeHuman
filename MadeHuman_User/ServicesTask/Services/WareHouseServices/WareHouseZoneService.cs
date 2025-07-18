using System.Net.Http;
using System.Net.Http.Json;
using Madehuman_User.ViewModel.WareHouse;

namespace MadeHuman_User.ServicesTask.Services.WareHouseService
{
    public interface IWarehouseZoneApiService
    {
        Task<List<WareHouseZoneViewModel>?> GetAllAsync();
        Task<WareHouseZoneViewModel?> GetByIdAsync(Guid id);
        Task<WareHouseZoneViewModel?> CreateAsync(WareHouseZoneViewModel model);
        Task<bool> UpdateAsync(Guid id, WareHouseZoneViewModel model);
    }
    public class WarehouseZoneApiService : IWarehouseZoneApiService
    {
        private readonly HttpClient _httpClient;

        public WarehouseZoneApiService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("API");
        }

        public async Task<List<WareHouseZoneViewModel>?> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<WareHouseZoneViewModel>>("api/WareHouseZone");
        }

        public async Task<WareHouseZoneViewModel?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<WareHouseZoneViewModel>($"api/WareHouseZone/{id}");
        }

        public async Task<WareHouseZoneViewModel?> CreateAsync(WareHouseZoneViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/WareHouseZone", model);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<WareHouseZoneViewModel>();
            return null;
        }

        public async Task<bool> UpdateAsync(Guid id, WareHouseZoneViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/WareHouseZone/{id}", model);
            return response.IsSuccessStatusCode;
        }
    }
}
