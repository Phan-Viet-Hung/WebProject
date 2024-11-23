using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            // Ràng buộc duy nhất cho Email
            builder.HasIndex(u => u.Email)
                   .IsUnique();

            // Ràng buộc duy nhất cho PhoneNumber
            builder.HasIndex(u => u.PhoneNumber)
                   .IsUnique();

            // Đặt độ dài tối đa cho chuỗi
            builder.Property(u => u.Email)
                   .HasMaxLength(100);

            builder.Property(u => u.PhoneNumber)
                   .HasMaxLength(15);
        }
    }
}
