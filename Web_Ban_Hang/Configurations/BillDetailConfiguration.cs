using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Configurations
{
    public class BillDetailConfiguration : IEntityTypeConfiguration<BillDetail>
    {
        public void Configure(EntityTypeBuilder<BillDetail> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<Bill>(x=>x.Bills).WithMany(x=>x.Details).HasForeignKey(x=>x.BillId);
            builder.HasOne<Product>(x => x.Products).WithMany(x => x.Details).HasForeignKey(x => x.ProductId);
        }
    }
}
