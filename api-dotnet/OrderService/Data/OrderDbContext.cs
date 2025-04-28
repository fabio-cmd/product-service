using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Order>(cfg =>
        {
            cfg.ToTable("ORDERS");
            cfg.HasKey(o => o.Id);
            cfg.Property(o => o.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            cfg.Property(o => o.ProductId).HasColumnName("PRODUCT_ID");
            cfg.Property(o => o.Quantity).HasColumnName("QTY");
            cfg.Property(o => o.Status)
               .HasConversion<string>()
               .HasMaxLength(20);
        });
    }
}
