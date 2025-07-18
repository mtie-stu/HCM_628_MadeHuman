using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Madehuman_User.ViewModel.WareHouse;

namespace MadeHuman_User.ServicesTask.Services.WareHouseService
{
    public interface IWarehouseApiService
    {
        Task<List<WareHouseViewModel>?> GetAllAsync();
        Task<WareHouseViewModel?> GetByIdAsync(Guid id);
        Task<WareHouseViewModel?> CreateAsync(WareHouseViewModel model);
        Task<bool> UpdateAsync(Guid id, WareHouseViewModel model);
    }

    public class WarehouseApiService : IWarehouseApiService
    {
        private readonly HttpClient _httpClient;

        public WarehouseApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<List<WareHouseViewModel>?> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<WareHouseViewModel>>("api/Warehouses");
        }

        public async Task<WareHouseViewModel?> GetByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<WareHouseViewModel>($"api/Warehouses/{id}");
        }

        public async Task<WareHouseViewModel?> CreateAsync(WareHouseViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Warehouses", model);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WareHouseViewModel>();
            }
            return null;
        }

        public async Task<bool> UpdateAsync(Guid id, WareHouseViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Warehouses/{id}", model);
            return response.IsSuccessStatusCode;
        }
    }
}
