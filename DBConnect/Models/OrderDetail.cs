namespace DBConnect.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Lưu giá tại thời điểm mua (vì giá máy có thể thay đổi sau này)

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}