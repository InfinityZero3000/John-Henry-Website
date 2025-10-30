using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JohnHenryFashionWeb.TempModels;

public partial class JohnhenryDbContext : DbContext
{
    public JohnhenryDbContext(DbContextOptions<JohnhenryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ShippingMethods> ShippingMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShippingMethods>(entity =>
        {
            entity.HasIndex(e => e.Code, "IX_ShippingMethods_Code").IsUnique();

            entity.HasIndex(e => e.IsActive, "IX_ShippingMethods_IsActive");

            entity.HasIndex(e => e.SortOrder, "IX_ShippingMethods_SortOrder");

            entity.Property(e => e.AvailableRegions).HasMaxLength(1000);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Cost).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MaxWeight).HasPrecision(10, 2);
            entity.Property(e => e.MinOrderAmount).HasPrecision(18, 2);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
