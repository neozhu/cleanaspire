// This class configures the database schema for the Product entity.
// It implements IEntityTypeConfiguration<Product> to define the entity's properties, constraints, and relationships at the database level.

// Purpose:
// 1. Map the Product entity to the database with specific configurations for its properties.
// 2. Ensure the database enforces data integrity (e.g., unique constraints, required fields).
// 3. Customize how certain properties are stored in the database (e.g., converting enums to strings).
// 4. Exclude non-persistent properties (e.g., DomainEvents) from being mapped to the database.

using CleanAspire.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanAspire.Infrastructure.Persistence.Configurations;
/// <summary>
/// Configures the Product entity.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the properties and relationships of the Product entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the Product entity.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Id).HasMaxLength(50);
        // Configures the Category property to be stored as a string in the database.
        builder.Property(x => x.Category).HasConversion<string>();

        // Configures the Name property to have a unique index.
        builder.HasIndex(x => x.Name).IsUnique();

        // Configures the Name property to have a maximum length of 80 characters and to be required.
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired();

        // Ignores the DomainEvents property so it is not mapped to the database.
        builder.Ignore(e => e.DomainEvents);
    }
}
