using Microsoft.EntityFrameworkCore;

using StripeDemo.Models;

namespace StripeDemo.Data;

public class StoreContext(
    DbContextOptions<StoreContext> options
) : DbContext(options) {

    public DbSet<Product> Products { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.PaymentIntentId);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Product)
            .WithMany()
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
