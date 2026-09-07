using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

/// <summary>
/// Entity type configuration for the <see cref="OsmTag"/> entity.
/// </summary>
public class OsmTagConfiguration : IEntityTypeConfiguration<OsmTag>
{
    /// <summary>
    /// Configures the OsmTag entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<OsmTag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Key)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.Value)
            .IsRequired()
            .HasMaxLength(2000);

        // EntityId - polymorphic FK (not enforced at DB level due to TPC)
        builder.Property(t => t.EntityId)
            .IsRequired(false);

        // EntityType - discriminator for polymorphic relationship
        builder.Property(t => t.EntityType)
            .HasMaxLength(20)
            .IsRequired(false);

        // Indexes for common queries
        builder.HasIndex(t => t.Key);
        builder.HasIndex(t => new { t.Key, t.Value });
        builder.HasIndex(t => new { t.EntityId, t.EntityType });
    }
}
