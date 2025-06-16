using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComboController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComboController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/combo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Combo>>> GetAll()
        {
            return await _context.Combos
                .Include(c => c.ComboItems)
                .Include(c => c.ProductSKUs)
                .ToListAsync();
        }

        // GET: api/combo/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Combo>> GetById(Guid id)
        {
            var combo = await _context.Combos
                .Include(c => c.ComboItems)
                .Include(c => c.ProductSKUs)
                .FirstOrDefaultAsync(c => c.ComboId == id);

            if (combo == null)
                return NotFound();

            return combo;
        }

        // POST: api/combo
        [HttpPost]
        public async Task<ActionResult<Combo>> Create([FromBody] Combo combo)
        {
            combo.ComboId = Guid.NewGuid();
            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = combo.ComboId }, combo);
        }

        // PUT: api/combo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Combo combo)
        {
            if (id != combo.ComboId)
                return BadRequest("ID không khớp");

            var existing = await _context.Combos.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Cập nhật thuộc tính
            existing.Name = combo.Name;
            existing.Description = combo.Description;
            existing.Discount = combo.Discount;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/combo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null)
                return NotFound(new { message = "Không tìm thấy combo!" });

            _context.Combos.Remove(combo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xoá combo thành công!" });
        }
    }
}
