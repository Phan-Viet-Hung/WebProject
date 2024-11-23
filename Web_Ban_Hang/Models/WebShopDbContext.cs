using Microsoft.EntityFrameworkCore;
using Web_Ban_Hang.Configurations;

namespace Web_Ban_Hang.Models
{
    public class WebShopDbContext : DbContext
    {
        public WebShopDbContext()
        {
        }

        public WebShopDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<BillItem> BillItems { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        //public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=LAP4HUNG; Database=WebShop_04;Trusted_Connection= True;" + "TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Áp dụng cấu hình cho User
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
