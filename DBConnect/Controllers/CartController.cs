using DBConnect.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBConnect.Models;
[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var carts = await _context.Carts
            .Include(c => c.Items)
            .ToListAsync();

        return Ok(carts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cart == null)
            return NotFound();

        return Ok(cart);
    }
    // Thêm đoạn này vào bên dưới hàm GetById nhé
    [HttpPost]
    public async Task<IActionResult> Create(Cart cart)
    {
        // Kiểm tra xem User đã có giỏ hàng chưa (thường 1 user chỉ có 1 giỏ)
        var existingCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == cart.UserId);
        if (existingCart != null)
        {
            return BadRequest("User này đã có giỏ hàng rồi!");
        }

        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = cart.Id }, cart);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cart = await _context.Carts.FindAsync(id);
        if (cart == null) return NotFound();

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}