namespace DBConnect.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Shipping, Completed, Cancelled

        public User? User { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}