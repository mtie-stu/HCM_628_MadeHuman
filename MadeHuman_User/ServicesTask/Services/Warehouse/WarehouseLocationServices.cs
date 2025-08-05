using Madehuman_Share.ViewModel.WareHouse;

namespace MadeHuman_User.ServicesTask.Services.Warehouse
{
    public interface IWarehouseLookupApiService
    {
        Task<WarehouseLocationInfoViewModel?> GetLocationInfoAsync(Guid warehouseLocationId);
    }

    public class WarehouseLocationServices : IWarehouseLookupApiService
    {
        private readonly HttpClient _httpClient;

        public WarehouseLocationServices(IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<WarehouseLocationInfoViewModel?> GetLocationInfoAsync(Guid warehouseLocationId)
        {
            var response = await _httpClient.GetAsync($"api/WarehouseLocation/location/{warehouseLocationId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<WarehouseLocationInfoViewModel>();
        }
    }
}
