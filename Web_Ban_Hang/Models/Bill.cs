using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Ban_Hang.Models
{
    public class Bill
    {
        [Key]
        public Guid Id { get; set; } // ID hóa đơn

        [Required]
        public Guid? UserId { get; set; } // ID người dùng (Customer)

        [ForeignKey("UserId")]
        public User? User { get; set; } // Navigation property để truy cập User

        [Required]
        public DateTime CreateDate { get; set; } // Ngày tạo hóa đơn

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Tổng số tiền

        [Required]
        public string? Status { get; set; } // Trạng thái hóa đơn ("Pending", "Paid", "Shipped", etc.)

        public List<BillDetail> Details { get; set; } // Danh sách sản phẩm trong hóa đơn
    }
}
