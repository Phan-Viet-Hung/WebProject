using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Web_Ban_Hang.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CartId { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        [EmailAddress] 
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
        [Required]
        [Range(6, 100)]
        public int Age { get; set; }
        public int Role { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        public List<CartDetail> CartDetails { get; set; }
    }
}
