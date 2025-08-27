using Madehuman_Share.ViewModel.Outbound;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MadeHuman_User.ServicesTask.Services.OutboundService
{
    public interface IPickTaskApiService
    {
        Task<List<PickTaskViewModelForIndexView>> GetMyPickTasksAsync(HttpContext httpContext);
        Task<Guid?> AssignPickTaskAsync(HttpContext httpContext);
        Task<List<string>> ValidatePickTaskScanAsync(HttpContext httpContext, ScanPickTaskValidationRequest request);
        Task<List<string>> ConfirmPickDetailToBasketAsync(HttpContext httpContext, Guid pickTaskId, Guid pickTaskDetailId, Guid basketId);
        Task<PickTaskFullViewModel?> GetPickTaskDetailAsync(HttpContext httpContext, Guid id);
        Task<List<string>> ConfirmBasketAsync(HttpContext httpContext, ConfirmBasketRequest data);
    }

    public class PickTaskApiService : IPickTaskApiService
    {
        private readonly HttpClient _client;
        private readonly ILogger<PickTaskApiService> _logger;

        public PickTaskApiService(IHttpClientFactory httpClientFactory, ILogger<PickTaskApiService> logger)
        {
            _client = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        public async Task<Guid?> AssignPickTaskAsync(HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return null;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/PickTasks/assign");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("❌ Gọi API AssignPickTaskAsync thất bại: {StatusCode} - {Error}", response.StatusCode, error);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String)
                {
                    if (Guid.TryParse(idProp.GetString(), out var taskId))
                        return taskId;
                }

                _logger.LogInformation("📭 Phản hồi không chứa taskId: {Json}", json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi parse JSON phản hồi từ AssignPickTaskAsync");
            }

            return null;
        }


        public async Task<List<PickTaskViewModelForIndexView>> GetMyPickTasksAsync(HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
                throw new Exception("Không có JWT");

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/PickTasks/mine");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<PickTaskViewModelForIndexView>>();
            return result ?? new();
        }

        public async Task<List<string>> ValidatePickTaskScanAsync(HttpContext httpContext, ScanPickTaskValidationRequest data)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return new();
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/PickTasks/scan")
            {
                Content = JsonContent.Create(data)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return errorResult?.Errors ?? new() { "❌ Không xác định lỗi." };
            }

            var successResult = await response.Content.ReadFromJsonAsync<SuccessResponse>();
            return successResult?.Messages ?? new();
        }

        public async Task<List<string>> ConfirmPickDetailToBasketAsync(HttpContext httpContext, Guid pickTaskId, Guid pickTaskDetailId, Guid basketId)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return new();
            }

            var url = $"/api/PickTasks/confirm?pickTaskId={pickTaskId}&pickTaskDetailId={pickTaskDetailId}&basketId={basketId}";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return errorResult?.Errors ?? new() { "❌ Không xác định lỗi." };
            }

            var successResult = await response.Content.ReadFromJsonAsync<SuccessResponse>();
            return successResult?.Messages ?? new();
        }
        public async Task<PickTaskFullViewModel?> GetPickTaskDetailAsync(HttpContext httpContext, Guid id)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/PickTasks/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<PickTaskFullViewModel>();
            return result;
        }
        public async Task<List<string>> ConfirmBasketAsync(HttpContext httpContext, ConfirmBasketRequest data)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
                return new List<string> { "Không tìm thấy JWT." };

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/PickTasks/assign-basket")
            {
                Content = JsonContent.Create(data)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return error?.Errors ?? new() { "Lỗi không xác định." };
            }

            var success = await response.Content.ReadFromJsonAsync<SuccessResponse>();
            return success?.Messages ?? new();
        }

        private class SuccessResponse
        {
            public List<string> Messages { get; set; } = new();
        }

        private class ErrorResponse
        {
            public List<string> Errors { get; set; } = new();
        }
    }
}
