namespace Web_Ban_Hang.Models
{
    public class Cart
    {
        public Guid Id { get; set; } // ID giỏ hàng
        public string UserName { get; set; }
        public Guid UserId { get; set; } // ID người dùng (Customer)

        public Guid ProductId { get; set; } // ID sản phẩm
        public int Quantity { get; set; } // Số lượng sản phẩm
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        public List<CartDetail> CartDetails { get; set; }
        //public List<CartItem> CartItems { get; set; } // Danh sách sản phẩm trong giỏ hàng
    }
}
