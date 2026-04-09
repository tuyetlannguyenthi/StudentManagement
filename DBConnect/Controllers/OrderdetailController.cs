using System.Threading.Tasks;
using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Lấy chi tiết
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDetail>> Get(int id)
        {
            var od = await _context.OrderDetails
                .Include(d => d.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (od == null)
                return NotFound();

            return Ok(od);
        }

        // ✅ Update số lượng (AN TOÀN)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDetail updated)
        {
            if (id != updated.Id)
                return BadRequest("Id mismatch");

            var detail = await _context.OrderDetails
                .Include(d => d.Product)
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detail == null)
                return NotFound();

            if (updated.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0");

            var product = await _context.Products.FindAsync(detail.ProductId);
            if (product == null)
                return BadRequest("Product not found");

            // 👉 hoàn lại kho cũ
            product.Quantity += detail.Quantity;

            // 👉 check kho đủ không
            if (product.Quantity < updated.Quantity)
                return BadRequest("Not enough stock");

            // 👉 trừ kho mới
            product.Quantity -= updated.Quantity;

            // 👉 update quantity
            detail.Quantity = updated.Quantity;

            // 👉 update total order
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == detail.OrderId);

            if (order != null)
            {
                order.Total = order.OrderDetails
                    .Sum(d => d.Quantity * d.UnitPrice);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Xóa item khỏi order
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _context.OrderDetails
                .Include(d => d.Product)
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detail == null)
                return NotFound();

            // 👉 hoàn lại kho
            if (detail.Product != null)
            {
                detail.Product.Quantity += detail.Quantity;
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == detail.OrderId);

            _context.OrderDetails.Remove(detail);

            // 👉 update total
            if (order != null)
            {
                order.Total = order.OrderDetails
                    .Where(d => d.Id != id)
                    .Sum(d => d.Quantity * d.UnitPrice);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}