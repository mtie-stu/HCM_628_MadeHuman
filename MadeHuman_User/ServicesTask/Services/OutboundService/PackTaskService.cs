using Madehuman_Share.ViewModel.Outbound;

namespace MadeHuman_User.ServicesTask.Services.OutboundService
{
    public interface IPackTaskService
    {
        Task<AssignPackTaskResultViewModel?> AssignPackTaskAsync(Guid outboundTaskItemId, HttpContext httpContext);
    }
    public class PackTaskService : IPackTaskService
    {
        private readonly HttpClient _client;
        private readonly ILogger<PackTaskService> _logger;

        public PackTaskService(IHttpClientFactory httpClientFactory, ILogger<PackTaskService> logger)
        {
            _client = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        public async Task<AssignPackTaskResultViewModel?> AssignPackTaskAsync(Guid outboundTaskItemId, HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken trong cookie.");
                return null;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/PackTask/assign/{outboundTaskItemId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("❌ Gọi API PackTask thất bại: {StatusCode} - {Error}", response.StatusCode, error);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<AssignPackTaskResultViewModel>();
            return result;
        }
    }
}
