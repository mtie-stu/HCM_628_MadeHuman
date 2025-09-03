using Madehuman_Share.ViewModel.Inbound.InboundReceipt;

namespace MadeHuman_Admin.ServicesTask.Services.InboundService
{
    public interface IInboundReceiptService
    {
        Task<bool> CreateReceiptAsync(CreateInboundReceiptViewModel model);
        Task<List<InboundReceiptViewModel>> GetAllAsync();
        Task<InboundReceiptDetailViewModel?> GetByIdAsync(Guid id);
    }

    public class InboundReceiptService : IInboundReceiptService
    {
        private readonly HttpClient _client;

        public InboundReceiptService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("API");
        }
        public async Task<List<InboundReceiptViewModel>> GetAllAsync()
        {
            var requestUrl = "api/InboundReceipt"; // endpoint bạn đang gọi
            var fullUrl = new Uri(_client.BaseAddress, requestUrl);

            Console.WriteLine($"🔍 [DEBUG] Gọi API: {fullUrl}");

            var response = await _client.GetAsync(requestUrl);

            Console.WriteLine($"📡 [HTTP] Status: {(int)response.StatusCode} - {response.ReasonPhrase}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Không thành công khi gọi API!");
                return new List<InboundReceiptViewModel>();
            }

            var data = await response.Content.ReadFromJsonAsync<List<InboundReceiptViewModel>>();

            Console.WriteLine($"✅ Nhận được {data?.Count ?? 0} phiếu nhập.");
            return data ?? new List<InboundReceiptViewModel>();
        }

        public async Task<bool> CreateReceiptAsync(CreateInboundReceiptViewModel model)
        {
            var response = await _client.PostAsJsonAsync("api/InboundReceipt", model);
            return response.IsSuccessStatusCode;
        }
        public async Task<InboundReceiptDetailViewModel?> GetByIdAsync(Guid id)
        {
            var response = await _client.GetAsync($"api/InboundReceipt/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadFromJsonAsync<InboundReceiptDetailViewModel>();
            return data;
        }
    }
}
