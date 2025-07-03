using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Enums;

namespace LibraryManagement.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    // DbSet properties for each entity
    public DbSet<User> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure default values for timestamps
        modelBuilder.Entity<User>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<User>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Member>()
            .Property(e => e.MembershipDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Member>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Member>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Book>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Book>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Category>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Loan>()
            .Property(e => e.LoanDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Loan>()
            .Property(e => e.Status)
            .HasDefaultValue(LoanStatus.Borrowed);

        modelBuilder.Entity<Loan>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Loan>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Configure relationships
        modelBuilder.Entity<Member>()
            .HasOne(m => m.User)
            .WithOne(u => u.Member)
            .HasForeignKey<Member>(m => m.UserId);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.CreatedByUser)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.CreatedBy);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId);

        // Only configure what's NOT in data annotations
        ConfigureEnums(modelBuilder);
        ConfigureUniqueConstraints(modelBuilder);
        ConfigureDefaultValues(modelBuilder);
    }

    private void ConfigureEnums(ModelBuilder modelBuilder)
    {
        // Enums need explicit conversion (not in data annotations)
        modelBuilder.Entity<User>()
            .Property(e => e.Role)
            .HasConversion<int>();

        modelBuilder.Entity<Member>()
            .Property(e => e.MembershipStatus)
            .HasConversion<int>();

        modelBuilder.Entity<Loan>()
            .Property(e => e.Status)
            .HasConversion<int>();
    }

    private void ConfigureUniqueConstraints(ModelBuilder modelBuilder)
    {
        // Unique constraints (not in data annotations)
        modelBuilder.Entity<User>()
            .HasIndex(e => e.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(e => e.Email)
            .IsUnique();

        modelBuilder.Entity<Member>()
            .HasIndex(e => e.MemberNumber)
            .IsUnique();

        modelBuilder.Entity<Member>()
            .HasIndex(e => e.UserId)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasIndex(e => e.ISBN)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(e => e.Name)
            .IsUnique();
    }

    private void ConfigureDefaultValues(ModelBuilder modelBuilder)
    {
        // Default values (not in data annotations)
        modelBuilder.Entity<Book>()
            .Property(e => e.TotalCopies)
            .HasDefaultValue(1);

        modelBuilder.Entity<Book>()
            .Property(e => e.AvailableCopies)
            .HasDefaultValue(1);

        modelBuilder.Entity<Book>()
            .Property(e => e.IsActive)
            .HasDefaultValue(true);

        modelBuilder.Entity<Category>()
            .Property(e => e.IsActive)
            .HasDefaultValue(true);

        modelBuilder.Entity<Loan>()
            .Property(e => e.LoanDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Loan>()
            .Property(e => e.Status)
            .HasDefaultValue(LoanStatus.Borrowed);
    }
}