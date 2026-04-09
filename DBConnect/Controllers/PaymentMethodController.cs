using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentMethodController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _context.PaymentMethods.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(PaymentMethod p)
        {
            _context.PaymentMethods.Add(p);
            await _context.SaveChangesAsync();
            return Ok(p);
        }
    }
}
