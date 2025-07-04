using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Enums;
using LibraryManagement.Core.Configurations;
using LibraryManagement.Data.Configurations;

namespace LibraryManagement.Data.Configurations;

public class UserConfiguration : BaseAuditableConfiguration<User>
{
    protected override void ConfigureEntitySpecific(EntityTypeBuilder<User> builder)
    {
        builder.ConfigureAuditFields();
        // Only configure what CAN'T be done with data annotations
        
        // Enum conversion (can't be done with data annotations)
        builder.Property(u => u.Role)
            .HasConversion<int>();

        // Unique indexes (can't be done with data annotations in .NET 9)
        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // Relationships (can be done with data annotations, but using fluent API for consistency)
        builder.HasOne(u => u.Member)
            .WithOne(m => m.User)
            .HasForeignKey<Member>(m => m.UserId);

        builder.HasMany(u => u.Books)
            .WithOne(b => b.CreatedByUser)
            .HasForeignKey(b => b.CreatedBy);
    }
} 