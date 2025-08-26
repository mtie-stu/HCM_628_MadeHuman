using Madehuman_Share.ViewModel.Inbound;
using MadeHuman_User.ServicesTask.Services.ShopService;
using MadeHuman_User.ServicesTask.Services.Warehouse;
using System.Text.Json;

namespace MadeHuman_User.ServicesTask.Services.InboundService
{
    public interface IRefillTaskService
    {
        Task<bool> CreateRefillTaskAsync(RefillTaskFullViewModel model, HttpContext httpContext);
        Task<List<RefillTaskFullViewModel>> GetAllRefillTasksAsync(HttpContext httpContext);
        Task<RefillTaskFullViewModel?> GetByIdAsync(Guid id, HttpContext httpContext);
        Task<List<RefillTaskDetailWithHeaderViewModel>> GetAllDetailsAsync(HttpContext httpContext);
        Task<List<string>> ValidateRefillScanAsync(ScanRefillTaskValidationRequest request, HttpContext httpContext);



    }
    public class RefillTaskService : IRefillTaskService
    {
        private readonly HttpClient _client;
        private readonly ILogger<RefillTaskService> _logger;
        private readonly IWarehouseLookupApiService _locationService;
        private readonly IProductService _productService;
        public RefillTaskService(IHttpClientFactory httpClientFactory, ILogger<RefillTaskService> logger, IWarehouseLookupApiService locationService, IProductService productService)
        {
            _client = httpClientFactory.CreateClient("API"); // 🔧 dùng đúng client "API" như bạn
            _logger = logger;
            _locationService = locationService;
            _productService = productService;
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
                if (data == null) return null;

                // ✅ Bổ sung SKU + From/To Location Name
                foreach (var detail in data.Details)
                {
                    if (detail.ProductSKUId != null)
                    {
                        var skuInfo = await _productService.GetSKUInfoAsync(detail.ProductSKUId.Value);
                        if (skuInfo != null)
                            detail.SKU = skuInfo.SkuCode;
                    }

                    var from = await _locationService.GetLocationInfoAsync(detail.FromLocation);
                    if (from != null)
                        detail.FromLocationName = from.NameLocation;

                    var to = await _locationService.GetLocationInfoAsync(detail.ToLocation);
                    if (to != null)
                        detail.ToLocationName = to.NameLocation;
                }

                //// ✅ Gán email người tạo (nếu CreateBy là Guid userId)
                //if (Guid.TryParse(data.CreateBy, out var userId))
                //{
                //    var user = await _userService.GetUserByIdAsync(userId);
                //    data.CreateByName = user?.Email ?? data.CreateBy;
                //}

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
                if (data == null || !data.Any()) return new();
                // ✅ Dùng cache để tránh gọi lại với cùng 1 locationId
                var locationNameCache = new Dictionary<Guid, string>();

                foreach (var item in data)
                {
                    if (!locationNameCache.TryGetValue(item.FromLocation, out var fromName))
                    {
                        var fromInfo = await _locationService.GetLocationInfoAsync(item.FromLocation);
                        fromName = fromInfo?.NameLocation ?? "(Không rõ)";
                        locationNameCache[item.FromLocation] = fromName;
                    }

                    if (!locationNameCache.TryGetValue(item.ToLocation, out var toName))
                    {
                        var toInfo = await _locationService.GetLocationInfoAsync(item.ToLocation);
                        toName = toInfo?.NameLocation ?? "(Không rõ)";
                        locationNameCache[item.ToLocation] = toName;
                    }

                    item.FromLocationName = fromName;
                    item.ToLocationName = toName;
                }

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception khi gọi GetAllDetailsAsync");
                return new();
            }
        }

        public async Task<List<string>> ValidateRefillScanAsync(ScanRefillTaskValidationRequest request, HttpContext httpContext)
        {
            var jwt = httpContext.Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("❌ Không tìm thấy JWTToken.");
                return new() { "❌ Chưa đăng nhập." };
            }

            var requestMsg = new HttpRequestMessage(HttpMethod.Post, "/api/RefillTask/validate-scan")
            {
                Content = JsonContent.Create(request)
            };
            requestMsg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            try
            {
                var response = await _client.SendAsync(requestMsg);
                var result = await response.Content.ReadFromJsonAsync<List<string>>();
                return result ?? new() { "❌ Lỗi không xác định từ server." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi gọi ValidateRefillScanAsync");
                return new() { "❌ Lỗi kết nối tới server." };
            }
        }

    }
}
