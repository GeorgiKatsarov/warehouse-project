using Microsoft.EntityFrameworkCore;
using Warehouse.Api.Models;

namespace Warehouse.Api.Data;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Id)
                .UseIdentityColumn();

            entity.Property(p => p.Price)
                .HasPrecision(18, 2);

            entity.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
        });
    }
}