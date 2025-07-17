using Madehuman_Share.ViewModel.Inbound;

namespace MadeHuman_User.ServicesTask.Services.InboundService
{
    public interface IRefillTaskService
    {
        Task<bool> CreateRefillTaskAsync(RefillTaskFullViewModel model);
        Task<List<RefillTaskFullViewModel>> GetAllRefillTasksAsync();

    }
    public class RefillTaskService : IRefillTaskService
    {
        private readonly HttpClient _client;
        private readonly ILogger<RefillTaskService> _logger;
        public RefillTaskService(IHttpClientFactory httpClientFactory, ILogger<RefillTaskService> logger)
        {
            _client = httpClientFactory.CreateClient("API"); // 🔧 dùng đúng client "API" như bạn
            _logger = logger;
        }
        public async Task<List<RefillTaskFullViewModel>> GetAllRefillTasksAsync()
        {
            var response = await _client.GetAsync("/api/RefillTask");

            if (!response.IsSuccessStatusCode)
                return new List<RefillTaskFullViewModel>();

            var data = await response.Content.ReadFromJsonAsync<List<RefillTaskFullViewModel>>();
            return data ?? new List<RefillTaskFullViewModel>();
        }

        public async Task<bool> CreateRefillTaskAsync(RefillTaskFullViewModel model)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("/api/RefillTask", model);

                if (response.IsSuccessStatusCode)
                    return true;

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("❌ Tạo nhiệm vụ Refill thất bại: {Error}", error);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception khi gọi API RefillTask");
                return false;
            }
        }
    }
}
