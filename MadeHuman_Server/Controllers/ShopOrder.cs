using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShopOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/shoporder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShopOrder>>> GetAll()
        {
            return await _context.ShopOrders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        // GET: api/shoporder/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ShopOrder>> GetById(Guid id)
        {
            var order = await _context.ShopOrders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.ShopOrderId == id);

            if (order == null)
                return NotFound();

            return order;
        }

        // POST: api/shoporder
        [HttpPost]
        public async Task<ActionResult<ShopOrder>> Create([FromBody] ShopOrder order)
        {
            order.ShopOrderId = Guid.NewGuid();
            order.OrderDate = DateTime.UtcNow;

            _context.ShopOrders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.ShopOrderId }, order);
        }

        // PUT: api/shoporder/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ShopOrder updated)
        {
            if (id != updated.ShopOrderId)
                return BadRequest("ID không khớp");

            var existing = await _context.ShopOrders.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.TotalAmount = updated.TotalAmount;
            existing.Status = updated.Status;
            existing.AppUserId = updated.AppUserId;
            // Không cập nhật OrderDate

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/shoporder/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var order = await _context.ShopOrders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.ShopOrders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
