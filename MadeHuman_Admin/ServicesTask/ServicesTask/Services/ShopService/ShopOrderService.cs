using Madehuman_Share.ViewModel.Shop;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MadeHuman_Admin.ServicesTask.Services.ShopService
{
    public interface IShopOrderService
    {
        Task<List<ShopOrderListItemViewModel>> GetAllAsync();
        //Task<ShopOrderViewModel?> GetByIdAsync(Guid id);
        Task<HttpResponseMessage> CreateOrderAsync(CreateShopOrderWithMultipleItems model);
        Task<ShopOrderDetailViewModel?> GetOrderByIdAsync(Guid id);
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
            var response = await _client.GetAsync("api/ShopOrder");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<List<ShopOrderListItemViewModel>>(json, new JsonSerializerOptions
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
        public async Task<HttpResponseMessage> CreateOrderAsync(CreateShopOrderWithMultipleItems model)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/ShopOrder", content);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine("🔴 API trả về lỗi:");
                Console.WriteLine(err);
                Console.WriteLine("🟡 Order gửi lên:");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true }));

            }

            return response;
        }

        public async Task<ShopOrderDetailViewModel?> GetOrderByIdAsync(Guid id)
        {
            var response = await _client.GetAsync($"/api/ShopOrder/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var order = System.Text.Json.JsonSerializer.Deserialize<ShopOrderDetailViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return order;
        }
    }
}
