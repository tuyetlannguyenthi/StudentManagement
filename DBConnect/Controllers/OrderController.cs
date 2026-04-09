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
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Lấy tất cả đơn hàng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsNoTracking()
                .ToListAsync();

            return Ok(orders);
        }

        // ✅ Lấy đơn theo ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // ✅ Tạo đơn hàng
        [HttpPost]
        public async Task<ActionResult<Order>> Create([FromBody] Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!order.OrderDetails.Any())
                return BadRequest("Order must contain at least one item.");

            decimal total = 0;

            foreach (var item in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null)
                    return BadRequest($"Product {item.ProductId} not found");

                if (item.Quantity <= 0)
                    return BadRequest("Quantity must be greater than 0");

                if (product.Quantity < item.Quantity)
                    return BadRequest($"Not enough stock for product {product.Name}");

                // ✅ lấy giá từ DB (không tin client)
                item.UnitPrice = product.Price;

                // ✅ trừ kho
                product.Quantity -= item.Quantity;

                total += item.Quantity * item.UnitPrice;
            }

            order.Total = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        // ✅ Xóa đơn hàng
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            // ✅ hoàn lại kho khi xóa (optional nhưng nên có)
            foreach (var item in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                }
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Lấy đơn theo User
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByUser(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsNoTracking()
                .ToListAsync();

            return Ok(orders);
        }
    }
}