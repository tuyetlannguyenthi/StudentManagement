using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DBConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VoucherController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _context.Vouchers.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Voucher v)
        {
            _context.Vouchers.Add(v);
            await _context.SaveChangesAsync();
            return Ok(v);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _context.Vouchers.FindAsync(id);
            if (v == null) return NotFound();

            _context.Vouchers.Remove(v);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
