using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        b.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(255);

        b.Entity<Category>()
            .Property(c => c.Name)
            .HasMaxLength(100);

        b.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        b.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(20);
        b.Entity<Transaction>()
            .HasIndex(t => new { t.UserId, t.Date });

        // Deleting a user deletes their data
        b.Entity<Category>()
            .HasOne(c => c.User).WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Block deleting a category that still has transactions
        b.Entity<Transaction>()
            .HasOne(t => t.Category).WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Transaction>()
            .HasOne(t => t.User).WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}