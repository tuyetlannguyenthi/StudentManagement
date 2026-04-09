namespace DBConnect.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty; // Tên người nhận
        public string PhoneNumber { get; set; } = string.Empty;
        public string DetailAddress { get; set; } = string.Empty; // Số nhà, tên đường...
        public bool IsDefault { get; set; } // Địa chỉ mặc định

        public User? User { get; set; }
    }
}