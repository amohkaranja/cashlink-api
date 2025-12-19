using Microsoft.EntityFrameworkCore;
using CashLink.Api.Models;

namespace CashLink.Api.Data;

public class CashLinkDbContext : DbContext
{
    public CashLinkDbContext(DbContextOptions<CashLinkDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionRef).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SenderAccount).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ReceiverAccount).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => e.TransactionRef).IsUnique();
        });
    }
}
