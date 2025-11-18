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
    }
}
