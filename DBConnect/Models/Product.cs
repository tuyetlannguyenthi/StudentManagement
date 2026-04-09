using System.ComponentModel.DataAnnotations;

namespace DBConnect.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}