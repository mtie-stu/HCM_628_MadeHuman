using Madehuman_Share.ViewModel.Outbound;
using System.Net.Http.Headers;

namespace MadeHuman_User.ServicesTask.Services.OutboundService
{
    public interface IDispatchTaskService
    {
        Task<AssignDispatchTaskResultViewModel?> AssignDispatchTaskAsync(Guid outboundTaskItemId, HttpContext httpContext);
    }

    public class DispatchTaskService : IDispatchTaskService
    {
        private readonly HttpClient _client;
        private readonly ILogger<DispatchTaskService> _logger;

        public DispatchTaskService(IHttpClientFactory httpClientFactory, ILogger<DispatchTaskService> logger)
        {
            _client = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        public async Task<AssignDispatchTaskResultViewModel?> AssignDispatchTaskAsync(Guid outboundTaskItemId, HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return null;
            }

            // LƯU Ý: đảm bảo HttpClient đã có BaseAddress = https://localhost:7204
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/DispatchTask/assign/{outboundTaskItemId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("❌ Gọi API DispatchTask thất bại: {StatusCode} - {Error}", response.StatusCode, errorText);
                return new AssignDispatchTaskResultViewModel
                {
                    Logs = new() { $"❌ API lỗi {((int)response.StatusCode)}: {errorText}" }
                };
            }

            // 204 NoContent
            if ((int)response.StatusCode == 204)
                return new AssignDispatchTaskResultViewModel { Logs = new() { "ℹ️ API không trả nội dung." } };

            // Xử lý nhiều dạng body khác nhau
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
            if (!contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            {
                var raw = await response.Content.ReadAsStringAsync();
                return new AssignDispatchTaskResultViewModel { Logs = new() { raw } };
            }

            // 1) Thử parse đúng ViewModel
            try
            {
                var vm = await response.Content.ReadFromJsonAsync<AssignDispatchTaskResultViewModel>();
                if (vm != null) return vm;
            }
            catch { /* fallback tiếp */ }

            // 2) Thử parse List<string>
            try
            {
                var logs = await response.Content.ReadFromJsonAsync<List<string>>();
                if (logs != null) return new AssignDispatchTaskResultViewModel { Logs = logs };
            }
            catch { /* fallback tiếp */ }

            // 3) Thử parse object { message = "..."} phổ biến
            try
            {
                var obj = await response.Content.ReadFromJsonAsync<Dictionary<string, object?>>();
                if (obj != null)
                {
                    var list = new List<string>();
                    foreach (var kv in obj)
                        list.Add($"{kv.Key}: {kv.Value}");
                    return new AssignDispatchTaskResultViewModel { Logs = list };
                }
            }
            catch { /* cuối cùng trả raw */ }

            // 4) Cuối cùng: raw text
            var rawText = await response.Content.ReadAsStringAsync();
            return new AssignDispatchTaskResultViewModel { Logs = new() { rawText } };
        }

    }
}
