using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Controllers.Shop
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

       
    }
}
