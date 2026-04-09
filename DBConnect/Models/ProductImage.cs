namespace DBConnect.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public bool IsMain { get; set; } // Có phải ảnh đại diện không

        public Product? Product { get; set; }
    }
}