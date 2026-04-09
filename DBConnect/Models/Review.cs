namespace DBConnect.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; } // 1 đến 5 sao
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public Product? Product { get; set; }
        public User? User { get; set; }
    }
}