using Madehuman_Share.ViewModel.Inbound;
using System.Text.Json;

namespace MadeHuman_User.ServicesTask.Services.InboundService
{
    public interface IRefillTaskService
    {
        Task<bool> CreateRefillTaskAsync(RefillTaskFullViewModel model, HttpContext httpContext);
        Task<List<RefillTaskFullViewModel>> GetAllRefillTasksAsync(HttpContext httpContext);
        Task<RefillTaskFullViewModel?> GetByIdAsync(Guid id, HttpContext httpContext);
        Task<List<RefillTaskDetailWithHeaderViewModel>> GetAllDetailsAsync(HttpContext httpContext);


    }
    public class RefillTaskService : IRefillTaskService
    {
        private readonly HttpClient _client;
        private readonly ILogger<RefillTaskService> _logger;
        public RefillTaskService(IHttpClientFactory httpClientFactory, ILogger<RefillTaskService> logger)
        {
            _client = httpClientFactory.CreateClient("API"); // 🔧 dùng đúng client "API" như bạn
            _logger = logger;
        }
        public async Task<List<RefillTaskFullViewModel>> GetAllRefillTasksAsync(HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return new();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/RefillTask");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("❌ Gọi API RefillTask thất bại: {StatusCode} - {Error}", response.StatusCode, error);
                return new();
            }

            var data = await response.Content.ReadFromJsonAsync<List<RefillTaskFullViewModel>>();
            return data ?? new();
        }



        public async Task<bool> CreateRefillTaskAsync(RefillTaskFullViewModel model, HttpContext httpContext)
        {
            try
            {
                var jwt = httpContext.Request.Cookies["JWTToken"];
                if (string.IsNullOrEmpty(jwt))
                {
                    _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                    return false;
                }

                var requestUri = "/api/RefillTask";

                var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = JsonContent.Create(model)
                };
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                _logger.LogInformation("📤 Sending RefillTask request to {Uri} with payload: {Payload}", requestUri, JsonSerializer.Serialize(model));

                var response = await _client.SendAsync(request);

                _logger.LogInformation("📥 Response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✅ Tạo nhiệm vụ Refill thành công.");
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();

                _logger.LogWarning("❌ Tạo nhiệm vụ Refill thất bại.");
                _logger.LogWarning("❌ StatusCode: {StatusCode}", response.StatusCode);
                _logger.LogWarning("❌ Response content: {Error}", errorContent);

                return false;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "❌ Lỗi HTTP khi gọi API RefillTask.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception không xác định khi gọi API RefillTask.");
                return false;
            }
        }
        public async Task<RefillTaskFullViewModel?> GetByIdAsync(Guid id, HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return null;
            }

            var requestUri = $"/api/RefillTask/{id}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            try
            {
                var response = await _client.SendAsync(request);
                _logger.LogInformation("📤 Gọi API GET {Uri} - Status: {StatusCode}", requestUri, response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("❌ Lỗi khi gọi API RefillTask/GetById: {Error}", error);
                    return null;
                }

                var data = await response.Content.ReadFromJsonAsync<RefillTaskFullViewModel>();
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception khi gọi GetByIdAsync RefillTask.");
                return null;
            }
        }
        public async Task<List<RefillTaskDetailWithHeaderViewModel>> GetAllDetailsAsync(HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return new();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/RefillTask/Alldetails");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            try
            {
                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("❌ Gọi API RefillTask/details thất bại: {StatusCode} - {Error}", response.StatusCode, error);
                    return new();
                }

                var data = await response.Content.ReadFromJsonAsync<List<RefillTaskDetailWithHeaderViewModel>>();
                return data ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception khi gọi GetAllDetailsAsync");
                return new();
            }
        }

    }
}
