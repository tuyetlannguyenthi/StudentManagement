using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DBConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var data = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ToListAsync();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Wishlist w)
        {
            _context.Wishlists.Add(w);
            await _context.SaveChangesAsync();
            return Ok(w);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var w = await _context.Wishlists.FindAsync(id);
            if (w == null) return NotFound();

            _context.Wishlists.Remove(w);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
