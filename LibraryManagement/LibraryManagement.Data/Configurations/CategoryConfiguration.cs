using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Configurations;
using LibraryManagement.Data.Configurations;

namespace LibraryManagement.Data.Configurations;

public class CategoryConfiguration : BaseAuditableConfiguration<Category>
{
    protected override void ConfigureEntitySpecific(EntityTypeBuilder<Category> builder)
    {
        builder.ConfigureAuditFields();
        // Only configure what CAN'T be done with data annotations
        
        // Default values
        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        // Unique indexes (can't be done with data annotations in .NET 9)
        builder.HasIndex(c => c.Name)
            .IsUnique();

        // Relationships
        builder.HasMany(c => c.Books)
            .WithOne(b => b.Category)
            .HasForeignKey(b => b.CategoryId);
    }
} 