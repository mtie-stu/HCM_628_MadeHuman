using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/orderitem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetAll()
        {
            return await _context.OrderItems
                .Include(o => o.ProductSKU)
                .Include(o => o.ShopOrder)
                .ToListAsync();
        }

        // GET: api/orderitem/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetById(Guid id)
        {
            var orderItem = await _context.OrderItems
                .Include(o => o.ProductSKU)
                .Include(o => o.ShopOrder)
                .FirstOrDefaultAsync(o => o.OrderItemId == id);

            if (orderItem == null)
                return NotFound();

            return orderItem;
        }

        // POST: api/orderitem
        [HttpPost]
        public async Task<ActionResult<OrderItem>> Create([FromBody] OrderItem item)
        {
            item.OrderItemId = Guid.NewGuid();
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.OrderItemId }, item);
        }

        // PUT: api/orderitem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderItem updatedItem)
        {
            if (id != updatedItem.OrderItemId)
                return BadRequest("ID không khớp");

            var existing = await _context.OrderItems.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Quantity = updatedItem.Quantity;
            existing.UnitPrice = updatedItem.UnitPrice;
            
            existing.ShopOrderId = updatedItem.ShopOrderId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/orderitem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.OrderItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
