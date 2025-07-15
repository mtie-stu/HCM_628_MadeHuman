using Madehuman_Share.ViewModel.Inbound;
using System.Net.Http.Headers;

namespace MadeHuman_Admin.ServicesTask.Services.InboundService
{
    public interface IInboundTaskService 
    {
        Task<bool> CreateAsync(Guid receiptId, HttpContext httpContext);
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


    }
}
