using Madehuman_Share.ViewModel.Outbound;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace MadeHuman_User.ServicesTask.Services.OutboundService
{
    public interface IBillRenderService
    {
        Task<List<PrintBillViewModel>> GetBillsByCheckTaskIdAsync(Guid checkTaskId, HttpContext httpContext);
    }
    public class BillRenderService : IBillRenderService
    {

        private readonly HttpClient _client;
        private readonly ILogger<BillRenderService> _logger;

        public BillRenderService(IHttpClientFactory httpClientFactory, ILogger<BillRenderService> logger)
        {
            _client = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        public async Task<List<PrintBillViewModel>> GetBillsByCheckTaskIdAsync(Guid checkTaskId, HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken.");
                return new();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Bill/print-bills/{checkTaskId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("❌ Gọi API print-bills thất bại: {StatusCode} - {Error}", response.StatusCode, error);
                return new();
            }

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonSerializer.Deserialize<List<PrintBillViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi parse JSON trả về từ Bill API.");
                return new();
            }
        }
    }
}
