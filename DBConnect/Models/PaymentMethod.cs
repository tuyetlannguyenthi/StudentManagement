namespace DBConnect.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ví dụ: COD, MoMo, Chuyển khoản
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}