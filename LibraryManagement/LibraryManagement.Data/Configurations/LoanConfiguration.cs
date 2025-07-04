using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryManagement.Core.Models;
using LibraryManagement.Core.Enums;
using LibraryManagement.Core.Configurations;
using LibraryManagement.Data.Configurations;

namespace LibraryManagement.Data.Configurations;

public class LoanConfiguration : BaseAuditableConfiguration<Loan>
{
    protected override void ConfigureEntitySpecific(EntityTypeBuilder<Loan> builder)
    {
        builder.ConfigureAuditFields();
        // Only configure what CAN'T be done with data annotations
        
        // Enum conversion
        builder.Property(l => l.Status)
            .HasConversion<int>();

        // Default values
        builder.Property(l => l.LoanDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(l => l.Status)
            .HasDefaultValue(LoanStatus.Borrowed);

        // Relationships
        builder.HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId);

        builder.HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId);
    }
} 