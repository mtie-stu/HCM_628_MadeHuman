using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    [Route("proxy-image")]
    public class ProxyImageController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("URL is required.");

            try
            {
                var client = new HttpClient();

                // Đảm bảo Google Drive redirect đến ảnh thực
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";

                return File(bytes, contentType);
            }
            catch
            {
                return NotFound("Không thể tải ảnh từ Google Drive.");
            }
        }
    }

}
