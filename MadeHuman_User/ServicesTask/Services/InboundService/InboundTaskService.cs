using Madehuman_Share.ViewModel.Inbound;
using System.Net.Http.Headers;

namespace MadeHuman_User.ServicesTask.Services.InboundService
{
    public interface IInboundTaskService 
    {
        Task<bool> CreateAsync(Guid receiptId, HttpContext httpContext);
        Task<List<InboundTaskViewModel>> GetAllAsync(string token);
        Task<(bool Success, string? Message, List<string>? Errors)> ValidateScanAsync(ScanInboundTaskValidationRequest request, HttpContext httpContext);
    }

    public class InboundTaskService : IInboundTaskService
    {
        private readonly HttpClient _client;

        public InboundTaskService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("API");
        }
        public async Task<bool> CreateAsync(Guid receiptId, HttpContext httpContext)
        {
            var vm = new CreateInboundTaskViewModel
            {
                InboundReceiptId = receiptId
            };

            // Lấy token từ cookie
            var token = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token không tồn tại.");
                return false;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/InboundTask/create")
            {
                Content = JsonContent.Create(vm)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return true;

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error creating inbound task: " + error);

            return false;
        }
        public async Task<(bool Success, string? Message, List<string>? Errors)> ValidateScanAsync( ScanInboundTaskValidationRequest request, HttpContext httpContext)  
        {
            var token = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(token))
                return (false, null, new List<string> { "❌ Không tìm thấy token người dùng." });

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/InboundTask/validate-scan")
            {
                Content = JsonContent.Create(request)
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ValidateScanSuccessResponse>();
                return (true, result?.Message, null);
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<ValidateScanErrorResponse>();
                return (false, null, result?.Errors ?? new List<string> { "❌ Lỗi không xác định từ server." });
            }
        }
        public async Task<List<InboundTaskViewModel>> GetAllAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return new List<InboundTaskViewModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/InboundTask");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Lỗi khi gọi API GetAll: " + await response.Content.ReadAsStringAsync());
                return new List<InboundTaskViewModel>();
            }

            var data = await response.Content.ReadFromJsonAsync<List<InboundTaskViewModel>>();
            return data ?? new List<InboundTaskViewModel>();
        }


    }
}
