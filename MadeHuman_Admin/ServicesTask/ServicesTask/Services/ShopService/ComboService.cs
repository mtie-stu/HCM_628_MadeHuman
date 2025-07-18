using Madehuman_User.ViewModel.Shop;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MadeHuman_Admin.ServicesTask.Services.ShopService
{
    public interface IComboService
    {
        Task<List<ComboListItemViewModel>> GetAllAsync();
        Task<object?> CreateComboAsync(CreateComboViewModel model); // object hoặc trả về ViewModel tùy ý
        Task<ComboDetailViewModel?> GetByIdAsync(Guid id);
        Task<bool> AddComboItemsAsync(AddComboItemsRequest request);
    }
    public class ComboService : IComboService
    {
        private readonly HttpClient _client;

        public ComboService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("API");
        }
        public async Task<object?> CreateComboAsync(CreateComboViewModel model)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Name ?? ""), "Name");
            formData.Add(new StringContent(model.Description ?? ""), "Description");
            formData.Add(new StringContent(model.Price.ToString()), "Price");

            if (model.ImageFiles != null)
            {
                foreach (var file in model.ImageFiles)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    formData.Add(streamContent, "ImageFiles", file.FileName);
                }
            }

            var response = await _client.PostAsync("api/combo/create-base", formData);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result; // hoặc deserialize nếu muốn
            }

            return null;
        }
        public async Task<List<ComboListItemViewModel>> GetAllAsync()
        {
            var response = await _client.GetAsync("/api/Combo/api/combo");

            if (!response.IsSuccessStatusCode)
                return new List<ComboListItemViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var combos = JsonSerializer.Deserialize<List<ComboListItemViewModel>>(json, options);
            return combos ?? new List<ComboListItemViewModel>();
        }
        public async Task<ComboDetailViewModel?> GetByIdAsync(Guid id)
        {
            var response = await _client.GetAsync($"/api/Combo/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var combo = JsonSerializer.Deserialize<ComboDetailViewModel>(json, options);
            return combo;
        }
        public async Task<bool> AddComboItemsAsync(AddComboItemsRequest request)
        {
            var response = await _client.PostAsJsonAsync("/api/Combo/add-items", request);
            return response.IsSuccessStatusCode;
        }
    }
}
