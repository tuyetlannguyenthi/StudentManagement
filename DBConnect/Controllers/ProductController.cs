using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DBConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(AppDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            return Ok(products);
        }

        // ✅ GET BY ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // ✅ CREATE
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == product.CategoryId);

            if (!categoryExists)
                return BadRequest("Category does not exist");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created product {Id}", product.Id);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // ✅ UPDATE AN TOÀN
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product updated)
        {
            if (id != updated.Id)
                return BadRequest("Id mismatch");

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            if (updated.Price < 0 || updated.Quantity < 0)
                return BadRequest("Invalid price or quantity");

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == updated.CategoryId);

            if (!categoryExists)
                return BadRequest("Category does not exist");

            // 👉 update từng field
            product.Name = updated.Name;
            product.Price = updated.Price;
            product.Quantity = updated.Quantity;
            product.CategoryId = updated.CategoryId;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated product {Id}", id);

            return NoContent();
        }

        // ✅ DELETE
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.CartItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            if (product.CartItems.Any())
                return BadRequest("Cannot delete product in cart");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted product {Id}", id);

            return NoContent();
        }

        // ✅ DELETE MULTIPLE
        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Invalid id list");

            var products = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (!products.Any())
                return NotFound("No products found");

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted {Count} products", products.Count);

            return NoContent();
        }
        // ✅ Lọc sản phẩm theo Category
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            return Ok(products);
        }
    }
}