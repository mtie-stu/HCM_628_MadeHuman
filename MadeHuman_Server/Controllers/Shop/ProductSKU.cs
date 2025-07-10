using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Controllers.Shop
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductSKUController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductSKUController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/productsku
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductSKU>>> GetAll()
        {
            return await _context.ProductSKUs
                .Include(p => p.Combo)
                .Include(p => p.Product)
                .ToListAsync();
        }

        // GET: api/productsku/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductSKU>> GetById(Guid id)
        {
            var item = await _context.ProductSKUs
                .Include(p => p.Combo)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item == null)
                return NotFound();

            return item;
        }

       

        // PUT: api/productsku/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductSKU updated)
        {
            if (id != updated.Id)
                return BadRequest("ID không khớp");

            var existing = await _context.ProductSKUs.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.SKU = updated.SKU;
            existing.ProductId = updated.ProductId;
            existing.ComboId = updated.ComboId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

    
    }
}
