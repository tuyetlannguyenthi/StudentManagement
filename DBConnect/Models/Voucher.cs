namespace DBConnect.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // Ví dụ: KM2024
        public double DiscountValue { get; set; } // Giá trị giảm
        public bool IsPercentage { get; set; } // True nếu giảm theo %, False nếu giảm tiền mặt
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; } // Số lần tối đa được sử dụng
        public bool IsActive { get; set; } = true;
    }
}