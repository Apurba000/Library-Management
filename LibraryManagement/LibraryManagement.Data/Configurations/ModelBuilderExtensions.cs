using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations;

public static class ModelBuilderExtensions
{
    public static void ConfigureAuditFields<T>(this EntityTypeBuilder<T> builder)
        where T : class
    {
        if (typeof(T).GetProperty("CreatedAt") != null)
        {
            builder.Property("CreatedAt")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
        if (typeof(T).GetProperty("UpdatedAt") != null)
        {
            builder.Property("UpdatedAt")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
} 