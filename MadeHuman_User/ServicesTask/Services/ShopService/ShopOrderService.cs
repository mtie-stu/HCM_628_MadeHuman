using Madehuman_Share.ViewModel.Shop;
using System.Text.Json;

namespace MadeHuman_User.ServicesTask.Services.ShopService
{
    public interface IShopOrderService
    {
        Task<List<ShopOrderListItemViewModel>> GetAllAsync();
        //Task<ShopOrderViewModel?> GetByIdAsync(Guid id);
    }
    public class ShopOrderService : IShopOrderService
    {
        private readonly HttpClient _client;

        public ShopOrderService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("API");
        }
        public async Task<List<ShopOrderListItemViewModel>> GetAllAsync()
        {
            var response = await _client.GetAsync("api/shoporder");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ShopOrderListItemViewModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();
        }

        //public async Task<ShopOrderViewModel?> GetByIdAsync(Guid id)
        //{
        //    var response = await _client.GetAsync($"api/shoporder/{id}");
        //    if (!response.IsSuccessStatusCode) return null;

        //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    return await response.Content.ReadFromJsonAsync<ShopOrderViewModel>(options);
        //}
    }
}
