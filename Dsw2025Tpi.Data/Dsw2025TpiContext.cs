using System.Reflection.Metadata;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext: DbContext
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        #region Product
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.Property(e => e.Id)
            .IsRequired();

            e.Property(e => e.Sku)
            .HasMaxLength(6)
            .IsRequired();

            e.Property(e => e.InternalCode)
            .HasMaxLength(7)
            .IsRequired();

            e.Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();

            e.Property(e => e.Description)
            .HasMaxLength(80);

            e.Property(e => e.CurrentUnitPrice)
            .HasPrecision(15, 2)
            .IsRequired();

            e.Property(e => e.StockQuantity)
            .HasMaxLength(3)
            .IsRequired();
        });
        #endregion

        #region Order
        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.Property(e => e.Id)
            .IsRequired();

            e.Property(e => e.Date)
            .IsRequired();

            e.Property(e => e.ShippingAddress)
            .HasMaxLength(50)
            .IsRequired();

            e.Property(e => e.BillingAddress)
            .HasMaxLength(50)
            .IsRequired();

            e.Property(e => e.Notes)
            .HasMaxLength(80);

            e.Property(e => e.TotalAmount)
            .HasPrecision(15, 2)
            .IsRequired();
        });
        #endregion

        #region OrderItem
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("Order Items");
            e.Property(e => e.Id)
            .IsRequired();

            e.Property(e => e.Quantity)
            .HasMaxLength(5)
            .IsRequired();

            e.Property(e => e.UnitPrice)
            .HasPrecision(15, 2)
            .IsRequired();
        });
        #endregion

        #region Customer
        modelBuilder.Entity<Customer>(e =>
        {
            e.Property(e => e.Id)
            .IsRequired();

            e.Property(e => e.Email)
            .HasMaxLength(100)
            .IsRequired();

            e.Property(e => e.Name)
            .HasMaxLength(60)
            .IsRequired();

            e.Property(e => e.PhoneNumber)
            .HasMaxLength(15)
            .IsRequired();
        });
        #endregion
    }
}


