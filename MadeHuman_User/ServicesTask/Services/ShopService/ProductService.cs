
using Madehuman_User.ViewModel.Shop;
using System.Net.Http.Headers;

namespace MadeHuman_User.ServicesTask.Services.ShopService
{
    public interface IProductService
    {
        Task<List<ProductListItemViewModel>> GetAllAsync();
        Task<ProductDetailViewModel?> GetProductDetailAsync(Guid id);
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

        public async Task<List<ProductListItemViewModel>> GetAllAsync()
        {
            var res = await _client.GetAsync("api/Product");
            if (!res.IsSuccessStatusCode) return new();

            var products = await res.Content.ReadFromJsonAsync<List<ProductListItemViewModel>>();
            return products ?? new();
        }


        public async Task<ProductDetailViewModel?> GetProductDetailAsync(Guid id)
        {
            var res = await _client.GetAsync($"api/product/{id}");
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<ProductDetailViewModel>();
        }


        public async Task<bool> CreateAsync(CreateProduct_ProdcutSKU_ViewModel model)
        {
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(model.Name), "Name");
            form.Add(new StringContent(model.Description ?? ""), "Description");
            form.Add(new StringContent(model.Price.ToString()), "Price");
            form.Add(new StringContent(model.SKU ?? ""), "SKU");//quét mã sku
            //form.Add(new StringContent(model.QuantityInStock.ToString()), "QuantityInStock");
            form.Add(new StringContent(model.CategoryId.ToString()), "CategoryId");
            
            if (model.ImageFiles != null)
            {
                foreach (var file in model.ImageFiles)
                {
                    var stream = file.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentLength = file.Length;
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                    form.Add(fileContent, "ImageFiles", file.FileName);
                }
            }

            var response = await _client.PostAsync("api/Product", form);
            return response.IsSuccessStatusCode;
        }



        public async Task<bool> UpdateAsync(Guid id, CreateProduct_ProdcutSKU_ViewModel model)
        {
            var response = await _client.PutAsJsonAsync($"api/product/{id}", model);
            return response.IsSuccessStatusCode;
        }

    }
}
