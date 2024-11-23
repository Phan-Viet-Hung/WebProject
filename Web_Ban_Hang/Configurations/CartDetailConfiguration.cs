using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Configurations
{
    public class CartDetailConfiguration : IEntityTypeConfiguration<CartDetail>
    {
        public void Configure(EntityTypeBuilder<CartDetail> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<Cart>(x=>x.Carts).WithMany(x=>x.CartDetails).HasForeignKey(x=>x.CartId);
            builder.HasOne<User>(x=>x.Users).WithMany(x=>x.CartDetails).HasForeignKey(x=>x.UserId);
            //builder.HasOne<CartItem>(x=>x.CartItems).WithMany(x=>x.Details).HasForeignKey(x=>x.ItemsId);
        }
    }
}
