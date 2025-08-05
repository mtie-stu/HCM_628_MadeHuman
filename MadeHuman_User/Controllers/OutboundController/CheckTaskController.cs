using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Mvc;

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
    }
}
