using Madehuman_Share.ViewModel.PartTime_Task;
using Newtonsoft.Json;
using System.Text;

namespace MadeHuman_Admin.ServicesTask.Services
{
    public interface ICheckinCheckoutService
    {
        Task<bool> SubmitCheckinCheckoutAsync(Checkin_Checkout_Viewmodel model);
        Task<List<CheckInCheckOutTodayViewModel>> GetTodayLogsAsync();
    }
    public class CheckinCheckoutService : ICheckinCheckoutService
    {
        private readonly HttpClient _client;

        public CheckinCheckoutService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("API");
        }

        public async Task<bool> SubmitCheckinCheckoutAsync(Checkin_Checkout_Viewmodel model)
        {
            var response = await _client.PostAsJsonAsync("api/usertask/checkin-checkout", model);

            return response.IsSuccessStatusCode;
        }
        public async Task<List<CheckInCheckOutTodayViewModel>> GetTodayLogsAsync()
        {
            var response = await _client.GetAsync("api/usertask/today");

            if (!response.IsSuccessStatusCode)
                return new List<CheckInCheckOutTodayViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CheckInCheckOutTodayViewModel>>(json);
        }
    }
}

