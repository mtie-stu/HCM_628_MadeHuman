using Madehuman_Share.ViewModel.Shop;
using System.Net.Http.Headers;

namespace MadeHuman_User.ServicesTask.Services.ShopService
{
    public interface IProductService
    {
        Task<List<CreateProduct_ProdcutSKU_ViewModel>> GetAllAsync();
        Task<CreateProduct_ProdcutSKU_ViewModel?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(CreateProduct_ProdcutSKU_ViewModel model);
        Task<bool> UpdateAsync(Guid id, CreateProduct_ProdcutSKU_ViewModel model);
    }
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;

        public ProductService(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
        }

        public async Task<List<CreateProduct_ProdcutSKU_ViewModel>> GetAllAsync()
        {
            var res = await _client.GetAsync("api/Product");
            if (!res.IsSuccessStatusCode) return new();
            return await res.Content.ReadFromJsonAsync<List<CreateProduct_ProdcutSKU_ViewModel>>() ?? new();
        }

        public async Task<CreateProduct_ProdcutSKU_ViewModel?> GetByIdAsync(Guid id)
        {
            var res = await _client.GetAsync($"api/Product/{id}");
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<CreateProduct_ProdcutSKU_ViewModel>();
        }

        public async Task<bool> CreateAsync(CreateProduct_ProdcutSKU_ViewModel model)
        {
            using var form = new MultipartFormDataContent();

            // ❌ Không cần gửi các ID như ProductId, Id (của SKU), ProductItemId
            form.Add(new StringContent(model.Name), "Name");
            form.Add(new StringContent(model.Description ?? ""), "Description");
            form.Add(new StringContent(model.Price.ToString()), "Price");
            form.Add(new StringContent(model.SKU ?? ""), "SKU");
            form.Add(new StringContent(model.QuantityInStock.ToString()), "QuantityInStock");
            form.Add(new StringContent(model.CategoryId.ToString()), "CategoryId"); // vẫn cần vì đây là khóa ngoại nhập vào

            if (model.ImageFiles != null)
            {
                foreach (var file in model.ImageFiles)
                {
                    var stream = file.OpenReadStream();
                    form.Add(new StreamContent(stream)
                    {
                        Headers =
                {
                    ContentLength = file.Length,
                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                }
                    }, "ImageFiles", file.FileName);
                }
            }

            var response = await _client.PostAsync("api/Product", form);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateAsync(Guid id, CreateProduct_ProdcutSKU_ViewModel model)
        {
            var response = await _client.PutAsJsonAsync($"api/Product/{id}", model);
            return response.IsSuccessStatusCode;
        }
    }
}
