using Madehuman_Share.ViewModel.WareHouse;
using System.Net;
using System.Text.Json;

namespace MadeHuman_User.ServicesTask.Services.Warehouse
{
    public interface IWarehouseLocationServices
    {
        Task<WarehouseLocationInfoViewModel?> GetLocationInfoAsync(Guid warehouseLocationId);
        Task<WarehouseLocationViewModel> CreateAsync(WarehouseLocationViewModel model, CancellationToken ct = default);
        Task<WarehouseLocationViewModel> UpdateAsync(Guid id, WarehouseLocationViewModel model, CancellationToken ct = default);
        Task<WarehouseLocationViewModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<WarehouseLocationViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<List<WarehouseLocationViewModel>> GenerateLocationsAsync(GenerateWHLocationRequest request, CancellationToken ct = default);
        Task<List<WarehouseLocationOptionViewModel>> GetLocationOptionsAsync(Guid zoneId, string status, CancellationToken ct = default);
    }

    public class WarehouseLocationServices : IWarehouseLocationServices
    {
        private readonly HttpClient _httpClient;
        private const string BasePath = "api/WareHouseLocation";
        public WarehouseLocationServices(IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };


        public async Task<WarehouseLocationInfoViewModel?> GetLocationInfoAsync(Guid warehouseLocationId)
        {
            var response = await _httpClient.GetAsync($"api/WarehouseLocation/location/{warehouseLocationId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<WarehouseLocationInfoViewModel>();
        }
        // ========== Create ==========
        // POST /api/WareHouseLocation
        public async Task<WarehouseLocationViewModel> CreateAsync(WarehouseLocationViewModel model, CancellationToken ct = default)
        {
            using var res = await _httpClient.PostAsJsonAsync(BasePath, model, _jsonOpts, ct);
            res.EnsureSuccessStatusCode(); // 201 Created expected
            // API trả về body là WarehouseLocationViewModel (Id thật sự đã lưu)
            var created = await res.Content.ReadFromJsonAsync<WarehouseLocationViewModel>(_jsonOpts, ct);
            return created!;
        }

        // ========== Update ==========
        // PUT /api/WareHouseLocation/{id}
        public async Task<WarehouseLocationViewModel> UpdateAsync(Guid id, WarehouseLocationViewModel model, CancellationToken ct = default)
        {
            using var res = await _httpClient.PutAsJsonAsync($"{BasePath}/{id}", model, _jsonOpts, ct);

            if (res.StatusCode == HttpStatusCode.NotFound)
                throw new KeyNotFoundException("Warehouse location not found");

            res.EnsureSuccessStatusCode();
            var updated = await res.Content.ReadFromJsonAsync<WarehouseLocationViewModel>(_jsonOpts, ct);
            return updated!;
        }

        // ========== GetById ==========
        // GET /api/WareHouseLocation/{id}
        public async Task<WarehouseLocationViewModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var res = await _httpClient.GetAsync($"{BasePath}/{id}", ct);

            if (res.StatusCode == HttpStatusCode.NotFound)
                return null;

            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<WarehouseLocationViewModel>(_jsonOpts, ct);
        }

        // ========== GetAll ==========
        // GET /api/WareHouseLocation
        public async Task<List<WarehouseLocationViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            using var res = await _httpClient.GetAsync(BasePath, ct);
            res.EnsureSuccessStatusCode();

            var list = await res.Content.ReadFromJsonAsync<List<WarehouseLocationViewModel>>(_jsonOpts, ct);
            return list ?? new List<WarehouseLocationViewModel>();
        }

        // ========== Generate ==========
        // POST /api/WareHouseLocation/generate-locations
        public async Task<List<WarehouseLocationViewModel>> GenerateLocationsAsync(GenerateWHLocationRequest request, CancellationToken ct = default)
        {
            using var res = await _httpClient.PostAsJsonAsync($"{BasePath}/generate-locations", request, _jsonOpts, ct);

            // Backend đã validate và trả 400 nếu invalid; EnsureSuccessStatusCode sẽ ném lỗi giúp dev thấy rõ.
            res.EnsureSuccessStatusCode();

            var list = await res.Content.ReadFromJsonAsync<List<WarehouseLocationViewModel>>(_jsonOpts, ct);
            return list ?? new List<WarehouseLocationViewModel>();
        }
        public async Task<List<WarehouseLocationOptionViewModel>> GetLocationOptionsAsync(Guid zoneId, string status, CancellationToken ct = default)
        {
            var url = $"{BasePath}/options?zoneId={zoneId}&status={status}";
            using var res = await _httpClient.GetAsync(url, ct);
            res.EnsureSuccessStatusCode();
            var list = await res.Content.ReadFromJsonAsync<List<WarehouseLocationOptionViewModel>>(_jsonOpts, ct);
            return list ?? new List<WarehouseLocationOptionViewModel>();
        }
    }

}
