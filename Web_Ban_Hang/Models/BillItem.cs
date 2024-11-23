using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Ban_Hang.Models
{
    public class BillItem
    {
        [Key]
        public Guid Id { get; set; } // ID chi tiết hóa đơn (khóa chính)

        [Required(ErrorMessage = "ID hóa đơn là bắt buộc.")]
        [ForeignKey("Bill")]
        public Guid OrderId { get; set; } // ID hóa đơn (khóa ngoại liên kết đến bảng Bill)

        [Required(ErrorMessage = "ID sản phẩm là bắt buộc.")]
        [ForeignKey("Product")]
        public Guid ProductId { get; set; } // ID sản phẩm (khóa ngoại liên kết đến bảng Product)

        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 1.")]
        public int Quantity { get; set; } // Số lượng sản phẩm

        [Required(ErrorMessage = "Giá từng sản phẩm là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá từng sản phẩm phải lớn hơn 0.")]
        public decimal UnitPrice { get; set; } // Giá từng sản phẩm

        // Navigation properties
        public Bill Bill { get; set; } // Liên kết tới bảng Bill
        public Product Product { get; set; } // Liên kết tới bảng Product

        // Dữ liệu chi tiết hóa đơn (tuỳ chọn nếu cần mở rộng)
        public List<BillDetail> Details { get; set; }
    }
}
