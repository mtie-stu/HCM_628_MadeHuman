using Madehuman_Share.ViewModel.Inbound;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MadeHuman_User.ServicesTask.Services.InboundService
{
    public interface IInboundTaskService 
    {
        Task<bool> CreateAsync(Guid receiptId, HttpContext httpContext);
        Task<List<InboundTaskViewModel>> GetAllAsync(string token);
        Task<(bool Success, string? Message, List<string>? Errors)> ValidateScanAsync(ScanInboundTaskValidationRequest request, HttpContext httpContext);
        // ✅ Thêm hàm GetById
        Task<GetInboundTaskById_Viewmodel?> GetByIdAsync(Guid inboundTaskId, HttpContext httpContext);
        Task<Guid> GetTaskIdByReceiptAsync(Guid receiptId, HttpContext httpContext);
    }

    public class InboundTaskService : IInboundTaskService
    {
        private readonly HttpClient _client;
        private readonly ILogger<InboundTaskService> _logger; // ✅ thêm field


        public InboundTaskService(IHttpClientFactory httpClientFactory, ILogger<InboundTaskService> logger)
        {
            _client = httpClientFactory.CreateClient("API");
            _logger = logger;
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
        public async Task<GetInboundTaskById_Viewmodel?> GetByIdAsync(Guid inboundTaskId, HttpContext httpContext)
        {
            // Lấy JWT từ cookie
            var token = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ Không tìm thấy token người dùng khi gọi GetByIdAsync.");
                return null;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/InboundTask/{inboundTaskId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // API của bạn đang trả NotFound("Không tìm thấy InboundTask.")
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine("❌ Lỗi GetByIdAsync: " + err);
                return null;
            }

            // Map về ViewModel đã có sẵn ở share project
            var data = await response.Content.ReadFromJsonAsync<GetInboundTaskById_Viewmodel>();
            return data;
        }

        public async Task<Guid> GetTaskIdByReceiptAsync(Guid receiptId, HttpContext httpContext)
        {
            var token = httpContext?.Request?.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(token)) return Guid.Empty;

            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, $"/api/InboundTask/id-by-receipt/{receiptId}");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resp = await _client.SendAsync(req);

                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return Guid.Empty;

                resp.EnsureSuccessStatusCode();

                var mediaType = resp.Content.Headers.ContentType?.MediaType?.ToLowerInvariant();
                if (!string.IsNullOrEmpty(mediaType) && mediaType.StartsWith("text/plain"))
                {
                    var text = (await resp.Content.ReadAsStringAsync()).Trim();
                    return Guid.TryParse(text, out var idFromText) ? idFromText : Guid.Empty;
                }

                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty("taskId", out var idProp) &&
                    Guid.TryParse(idProp.GetString(), out var idFromJson))
                {
                    return idFromJson;
                }

                if (doc.RootElement.ValueKind == JsonValueKind.String &&
                    Guid.TryParse(doc.RootElement.GetString(), out var idFromString))
                {
                    return idFromString;
                }

                return Guid.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTaskIdByReceiptAsync failed. ReceiptId: {ReceiptId}", receiptId); // ✅ dùng logger
                return Guid.Empty;
            }
        }



    }
}
