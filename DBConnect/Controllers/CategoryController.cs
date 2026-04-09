using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Lấy tất cả category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();

            return Ok(categories);
        }

        // ✅ Lấy theo id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // ✅ Tạo mới
        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
        }

        // ✅ Update an toàn (không overwrite toàn bộ)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category updatedCategory)
        {
            if (id != updatedCategory.Id)
                return BadRequest("Id mismatch");

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            // chỉ update field cần thiết
            category.Name = updatedCategory.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Xóa category (có check liên kết)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            if (category.Products != null && category.Products.Any())
                return BadRequest("Cannot delete category with existing products");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}