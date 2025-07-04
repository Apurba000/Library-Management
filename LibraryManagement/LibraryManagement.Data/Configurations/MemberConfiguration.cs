using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Enums;
using LibraryManagement.Core.Configurations;
using LibraryManagement.Data.Configurations;

namespace LibraryManagement.Data.Configurations;

public class MemberConfiguration : BaseAuditableConfiguration<Member>
{
    protected override void ConfigureEntitySpecific(EntityTypeBuilder<Member> builder)
    {
        builder.ConfigureAuditFields();
        // Only configure what CAN'T be done with data annotations
        
        // Enum conversion
        builder.Property(m => m.MembershipStatus)
            .HasConversion<int>();

        // Default values
        builder.Property(m => m.MembershipDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Unique indexes (can't be done with data annotations in .NET 9)
        builder.HasIndex(m => m.MemberNumber)
            .IsUnique();

        builder.HasIndex(m => m.UserId)
            .IsUnique();

        // Relationships
        builder.HasOne(m => m.User)
            .WithOne(u => u.Member)
            .HasForeignKey<Member>(m => m.UserId);

        builder.HasMany(m => m.Loans)
            .WithOne(l => l.Member)
            .HasForeignKey(l => l.MemberId);
    }
} 