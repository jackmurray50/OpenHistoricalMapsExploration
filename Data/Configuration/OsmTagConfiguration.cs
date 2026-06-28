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

        // Foreign key relationship to OsmEntity
        builder.HasOne(t => t.Entity)
            .WithMany(e => e.Tags)
            .HasForeignKey(t => t.EntityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for common queries
        builder.HasIndex(t => t.Key);
        builder.HasIndex(t => new { t.Key, t.Value });
    }
}
