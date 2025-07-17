using MadeHuman_Server.Data;
using MadeHuman_Server.Data.Seed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Controllers
{
    [Route("api/seed")]
    [ApiController]
    public class SeedDemoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeedDemoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("outbound")]
        public async Task<IActionResult> SeedOutbound()
        {
            try
            {
                await DemoSeeder.SeedOutboundDemoDataAsync(_context);
                return Ok("✅ Đã seed dữ liệu demo cho kho Outbound thành công.");
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, $"❌ Lỗi khi seed dữ liệu (DbUpdateException): {inner}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Lỗi khi seed dữ liệu (Exception): {ex.Message}");
            }
        }

    }
}
