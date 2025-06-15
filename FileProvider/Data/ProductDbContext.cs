using Microsoft.EntityFrameworkCore;
using FileProvider.Models;

namespace FileProvider.Data;

public class ProductDbContext : DbContext
{
    // DbSet for ProductEntity table
    public DbSet<ProductEntity> Products { get; set; }

    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.Revenue)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(e => e.CompanyName).IsRequired();
            entity.Property(e => e.OrganizationNumber).IsRequired(false);
            entity.Property(e => e.Address).IsRequired();
            entity.Property(e => e.PostalCode).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.PhoneNumber).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.BusinessType).IsRequired();
            entity.Property(e => e.NumberOfEmployees).IsRequired();
            entity.Property(e => e.CEO).IsRequired();
            entity.Property(e => e.CustomerId).IsRequired(false);
            entity.Property(e => e.SoldUntil).IsRequired(false);
            entity.Property(e => e.ReservedUntil).IsRequired(false);
        });
    }
}
