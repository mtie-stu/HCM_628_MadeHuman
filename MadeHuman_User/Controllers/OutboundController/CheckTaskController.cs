using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MadeHuman_User.Controllers.OutboundController
{
    [Route("CheckTask")]

    public class CheckTaskController : Controller
    {
        /// <summary>
        /// View kiểm hàng dạng SingleSKU
        /// </summary>
        [HttpGet("SingleSKU")]
        public IActionResult SingleSKU()
        {
            return View();
        }
        [HttpGet("MixSKU")]
        public IActionResult MixSKU()
        {
            return View();
        }

        /// <summary>
        /// Giao diện quét basket mới (sau khi kiểm xong)
        /// </summary>
        [HttpGet("CheckTask/ScanBasket")]
        public IActionResult ScanBasket()
        {
            return View(); // 👉 bạn có thể làm view quét basket riêng ở đây
        }
        [HttpGet("PrintBills/{checkTaskId}")]
        public async Task<IActionResult> PrintBills(Guid checkTaskId, [FromServices] IHttpClientFactory httpFactory, [FromServices] IConfiguration config)
        {
            var jwt = Request.Cookies["JWTToken"];
            if (string.IsNullOrEmpty(jwt))
                return Content("<h3>Missing JWTToken cookie</h3>", "text/html");

            var apiBase ="https://hcm-628-madehuman-api.onrender.com";
            var url = $"{apiBase}/api/Bill/print-bills/{checkTaskId}";

            var client = httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var resp = await client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return Content($"<h3>BE error: {(int)resp.StatusCode}</h3>", "text/html");

            var json = await resp.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<PrintBillViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            string ExtractBody(string html)
            {
                if (string.IsNullOrWhiteSpace(html)) return "";
                var m = Regex.Match(html, "<body[^>]*>([\\s\\S]*?)</body>", RegexOptions.IgnoreCase);
                return m.Success ? m.Groups[1].Value : html; // nếu BE trả fragment thì dùng luôn
            }

            var model = items.Select(x => new PrintBillViewModel
            {
                OutboundTaskItemId = x.OutboundTaskItemId,
                HtmlContent = ExtractBody(x.HtmlContent)
            }).ToList();

            Response.Headers["Cache-Control"] = "no-store"; // tránh cache trang in
            return View("Print80x80", model);
        }
    }
}
