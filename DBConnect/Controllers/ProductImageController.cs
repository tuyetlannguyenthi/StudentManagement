using DBConnect.Data;
using DBConnect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductImageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            return Ok(await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductImage img)
        {
            _context.ProductImages.Add(img);
            await _context.SaveChangesAsync();
            return Ok(img);
        }
    }
}
