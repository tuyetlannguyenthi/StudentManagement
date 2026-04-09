using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CartItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartItemController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ Thêm sản phẩm vào giỏ
    [HttpPost]
    public async Task<IActionResult> AddItem(CartItem item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (item.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0");

        // check cart tồn tại
        var cartExists = await _context.Carts.AnyAsync(c => c.Id == item.CartId);
        if (!cartExists)
            return BadRequest("Cart does not exist");

        // check product tồn tại
        var productExists = await _context.Products.AnyAsync(p => p.Id == item.ProductId);
        if (!productExists)
            return BadRequest("Product does not exist");

        // check trùng sản phẩm
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(i => i.CartId == item.CartId
                                  && i.ProductId == item.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            _context.CartItems.Add(item);
        }

        await _context.SaveChangesAsync();

        return Ok(item);
    }

    // ✅ Cập nhật số lượng
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, CartItem updatedItem)
    {
        if (id != updatedItem.Id)
            return BadRequest("Id mismatch");

        var item = await _context.CartItems.FindAsync(id);
        if (item == null)
            return NotFound();

        if (updatedItem.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0");

        item.Quantity = updatedItem.Quantity;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ✅ Xóa 1 item
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.CartItems.FindAsync(id);

        if (item == null)
            return NotFound();

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ✅ Lấy item theo cart
    [HttpGet("cart/{cartId}")]
    public async Task<IActionResult> GetByCart(int cartId)
    {
        var items = await _context.CartItems
            .Where(i => i.CartId == cartId)
            .Include(i => i.Product)
            .AsNoTracking()
            .ToListAsync();

        return Ok(items);
    }

    // ✅ Tăng số lượng +1
    [HttpPut("increase/{id}")]
    public async Task<IActionResult> Increase(int id)
    {
        var item = await _context.CartItems.FindAsync(id);

        if (item == null)
            return NotFound();

        item.Quantity += 1;
        await _context.SaveChangesAsync();

        return Ok(item);
    }

    // ✅ Giảm số lượng -1
    [HttpPut("decrease/{id}")]
    public async Task<IActionResult> Decrease(int id)
    {
        var item = await _context.CartItems.FindAsync(id);

        if (item == null)
            return NotFound();

        item.Quantity -= 1;

        if (item.Quantity <= 0)
        {
            _context.CartItems.Remove(item);
        }

        await _context.SaveChangesAsync();

        return Ok(item);
    }
}