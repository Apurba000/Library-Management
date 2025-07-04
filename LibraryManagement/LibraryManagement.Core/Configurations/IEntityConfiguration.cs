using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Core.Configurations;

/// <summary>
/// Base interface for entity configurations that can be defined in Core
/// but implemented in Data layer
/// </summary>
public interface IEntityConfiguration<T> where T : class
{
    void Configure(EntityTypeBuilder<T> builder);
}

/// <summary>
/// Base configuration for entities with audit fields
/// </summary>
public abstract class BaseAuditableConfiguration<T> : IEntityTypeConfiguration<T> where T : class
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        ConfigureAuditFields(builder);
        ConfigureEntitySpecific(builder);
    }

    protected virtual void ConfigureAuditFields(EntityTypeBuilder<T> builder)
    {
        // This will be implemented in the Data layer where EF Core is available
        // For now, we'll let derived classes handle audit field configuration
    }

    protected abstract void ConfigureEntitySpecific(EntityTypeBuilder<T> builder);
} 