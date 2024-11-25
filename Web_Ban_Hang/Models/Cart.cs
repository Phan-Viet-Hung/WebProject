using System.ComponentModel.DataAnnotations;

namespace Web_Ban_Hang.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; } // ID giỏ hàng

        [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên người dùng không được vượt quá 100 ký tự.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "ID người dùng là bắt buộc.")]
        public Guid UserId { get; set; } // ID người dùng (Customer)

        [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
        public Guid ProductId { get; set; } // ID sản phẩm

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn 0.")]
        public int Quantity { get; set; } // Số lượng sản phẩm

        [Range(0.01, double.MaxValue, ErrorMessage = "Tổng số tiền phải lớn hơn 0.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Ngày tạo là bắt buộc.")]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

        // Các thuộc tính liên kết với chi tiết giỏ hàng
        public List<CartDetail> CartDetails { get; set; }
    }
}
