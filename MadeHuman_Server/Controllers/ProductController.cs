using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductItems)
                .Include(p => p.ProductSKU)
                .Include(p => p.ComboItems)
                .ToListAsync();
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductItems)
                .Include(p => p.ProductSKU)
                .Include(p => p.ComboItems)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            return product;
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            product.ProductId = Guid.NewGuid();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product updated)
        {
            if (id != updated.ProductId)
                return BadRequest("ID không khớp");

            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Price = updated.Price;
            existing.CategoryId = updated.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
