using DBConnect.Data;
using DBConnect.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DBConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _context.Brands.AsNoTracking().ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.Brands.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand model)
        {
            _context.Brands.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Brand model)
        {
            var item = await _context.Brands.FindAsync(id);
            if (item == null) return NotFound();

            item.Name = model.Name;
            item.Description = model.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Brands.FindAsync(id);
            if (item == null) return NotFound();

            _context.Brands.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
