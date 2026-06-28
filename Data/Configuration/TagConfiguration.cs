using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

/// <summary>
/// Entity type configuration for the <see cref="Tag"/> entity.
/// </summary>
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    /// <summary>
    /// Configures the Tag entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Tag> builder)
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
