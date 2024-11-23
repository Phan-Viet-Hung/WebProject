using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_Ban_Hang.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; } // Khóa chính của bảng sản phẩm

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string ProductName { get; set; } // Tên sản phẩm

        [StringLength(500, ErrorMessage = "Mô tả sản phẩm không được vượt quá 500 ký tự.")]
        public string Description { get; set; } // Mô tả sản phẩm

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải là số không âm.")]
        public int Quantity { get; set; } // Số lượng sản phẩm

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal Price { get; set; } // Giá sản phẩm

        [Url(ErrorMessage = "Ảnh sản phẩm phải là một URL hợp lệ.")]
        public string Image { get; set; } // Link ảnh sản phẩm
        public string Status { get; set; }
        public List<BillDetail> Details { get; set; }
        public List<CartDetail> CartDetails { get; set; } // Liên kết tới chi tiết giỏ hàng
    }
}
