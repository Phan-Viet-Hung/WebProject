using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Web_Ban_Hang.Models
{
    public class CartDetail
    {
        [Key]
        public Guid Id { get; set; } // ID của bản ghi liên kết

        // ForeignKey từ Cart và User
        [ForeignKey("Cart")]
        public Guid? CartId { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public Guid? ProductId { get; set; }


        // Navigation properties
        public Cart? Carts { get; set; }
        public User? Users { get; set; }
        public Product? Products { get; set; }
    }
}
