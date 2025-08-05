using Madehuman_Share.ViewModel.Outbound;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace MadeHuman_User.ServicesTask.Services.OutboundService
{
    public interface ICheckTaskServices
    {
        Task<List<string>> ValidateSingleSKUCheckTaskAsync(SingleSKUCheckTaskRequest request, HttpContext httpContext);
    }
    public class CheckTaskServices : ICheckTaskServices
    {
        private readonly HttpClient _client;
        private readonly ILogger<CheckTaskServices> _logger;
        public CheckTaskServices(IHttpClientFactory httpClientFactory, ILogger<CheckTaskServices> logger)
        {
            _client = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        public async Task<List<string>> ValidateSingleSKUCheckTaskAsync(SingleSKUCheckTaskRequest request, HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return new() { "❌ Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn." };
            }

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/CheckTask/single-sku");
            httpRequest.Content = content;
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("❌ Gọi API ValidateSingleSKUCheckTaskAsync thất bại: {StatusCode} - {Error}", response.StatusCode, error);
                return new() { $"❌ Lỗi xác nhận SKU: {error}" };
            }

            try
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<string>>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new() { "❌ Không có phản hồi từ server." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi parse JSON phản hồi từ ValidateSingleSKUCheckTaskAsync");
                return new() { "❌ Lỗi định dạng phản hồi từ server." };
            }
        }
    }
}