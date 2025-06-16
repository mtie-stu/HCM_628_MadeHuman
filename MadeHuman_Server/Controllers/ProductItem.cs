using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/productitem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductItem>>> GetAll()
        {
            return await _context.ProductItems
                .Include(p => p.Product)
                .Include(p => p.OrderItems)
                .ToListAsync();
        }

        // GET: api/productitem/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductItem>> GetById(Guid id)
        {
            var item = await _context.ProductItems
                .Include(p => p.Product)
                .Include(p => p.OrderItems)
                .FirstOrDefaultAsync(p => p.ProductItemId == id);

            if (item == null)
                return NotFound();

            return item;
        }

        // POST: api/productitem
        [HttpPost]
        public async Task<ActionResult<ProductItem>> Create([FromBody] ProductItem item)
        {
            item.ProductItemId = Guid.NewGuid();
            _context.ProductItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.ProductItemId }, item);
        }

        // PUT: api/productitem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductItem updated)
        {
            if (id != updated.ProductItemId)
                return BadRequest("ID không khớp");

            var existing = await _context.ProductItems.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.SKU = updated.SKU;
            existing.QuantityInStock = updated.QuantityInStock;
            existing.ProductId = updated.ProductId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/productitem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.ProductItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.ProductItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
