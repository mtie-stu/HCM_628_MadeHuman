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

            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/DispatchTask/assign/{outboundTaskItemId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("❌ Gọi API DispatchTask thất bại: {StatusCode} - {Error}", response.StatusCode, error);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<AssignDispatchTaskResultViewModel>();
            return result;
        }
    }
}
