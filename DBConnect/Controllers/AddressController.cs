using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            return Ok(await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Address a)
        {
            _context.Addresses.Add(a);
            await _context.SaveChangesAsync();
            return Ok(a);
        }
    }
}
