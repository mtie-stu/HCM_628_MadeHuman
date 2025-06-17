using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MadeHuman_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComboItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComboItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/comboitem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComboItem>>> GetAll()
        {
            return await _context.ComboItems
                .Include(ci => ci.Combo)
                .Include(ci => ci.Product)
                .ToListAsync();
        }

        // GET: api/comboitem/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ComboItem>> GetById(Guid id)
        {
            var comboItem = await _context.ComboItems
                .Include(ci => ci.Combo)
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.ComboItemId == id);

            if (comboItem == null)
                return NotFound(new { message = "Không tìm thấy ComboItem!" });

            return comboItem;
        }

        // POST: api/comboitem
        [HttpPost]
        public async Task<ActionResult<ComboItem>> Create([FromBody] ComboItem comboItem)
        {
            comboItem.ComboItemId = Guid.NewGuid();
            _context.ComboItems.Add(comboItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = comboItem.ComboItemId }, comboItem);
        }

        // PUT: api/comboitem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ComboItem comboItem)
        {
            if (id != comboItem.ComboItemId)
                return BadRequest("ID không khớp");

            var existing = await _context.ComboItems.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Quantity = comboItem.Quantity;
            existing.ComboId = comboItem.ComboId;
            existing.ProductId = comboItem.ProductId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/comboitem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var comboItem = await _context.ComboItems.FindAsync(id);
            if (comboItem == null)
                return NotFound();

            _context.ComboItems.Remove(comboItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
