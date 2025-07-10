using MadeHuman_Server.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSheetController : ControllerBase
    {
        private readonly GoogleSheetService _sheetService;

        public GoogleSheetController(GoogleSheetService sheetService)
        {
            _sheetService = sheetService;
        }

        /// <summary>
        /// Đẩy toàn bộ dữ liệu PartTimeAssignment lên Google Sheet
        /// </summary>
        [HttpGet("sync-parttime")]
        public async Task<IActionResult> SyncPartTimeAssignment()
        {
            await _sheetService.SyncPartTimeAssignmentsAsync();
            return Ok("✅ Đã đồng bộ dữ liệu lên Google Sheet.");
        }
    }
}
