using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Configurations;
using LibraryManagement.Data.Configurations;

namespace LibraryManagement.Data.Configurations;

public class BookConfiguration : BaseAuditableConfiguration<Book>
{
    protected override void ConfigureEntitySpecific(EntityTypeBuilder<Book> builder)
    {
        builder.ConfigureAuditFields();
        // Only configure what CAN'T be done with data annotations
        
        // Default values (can be done with data annotations, but using fluent API for consistency)
        builder.Property(b => b.TotalCopies)
            .HasDefaultValue(1);

        builder.Property(b => b.AvailableCopies)
            .HasDefaultValue(1);

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        // Unique indexes (can't be done with data annotations in .NET 9)
        builder.HasIndex(b => b.ISBN)
            .IsUnique();

        // Relationships (can be done with data annotations, but using fluent API for consistency)
        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.CreatedByUser)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Loans)
            .WithOne(l => l.Book)
            .HasForeignKey(l => l.BookId);
    }
} 