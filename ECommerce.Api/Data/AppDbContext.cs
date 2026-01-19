using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasKey(a => a.ProductID);

            modelBuilder.Entity<Order>()
                .HasKey(a => a.OrderID);

            modelBuilder.Entity<Inventory>()
                .HasKey(a => a.InventoryID);

            modelBuilder.Entity<Inventory>()
                .Property(x => x.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Payment>()
                .HasKey(a => a.PaymentID);

            base.OnModelCreating(modelBuilder);
        }

    }
}