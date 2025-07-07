using Madehuman_Share.ViewModel.Shop;
using System.Text;
using System.Text.Json;

namespace MadeHuman_User.ServicesTask.Services.ShopService
{
    public interface IShopOrderService
    {
        Task<List<ShopOrderListItemViewModel>> GetAllAsync();
        //Task<ShopOrderViewModel?> GetByIdAsync(Guid id);
        Task<bool> CreateOrderAsync(CreateShopOrderWithMultipleItems model);
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
        public async Task<bool> CreateOrderAsync(CreateShopOrderWithMultipleItems model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("api/ShopOrder", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Lỗi khi gọi API: " + error);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi gửi request: " + ex.Message);
                return false;
            }
        }


    }
}
