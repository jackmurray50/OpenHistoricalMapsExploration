using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

/// <summary>
/// Entity type configuration for the <see cref="OsmEntity"/> base class.
/// </summary>
public class OsmEntityConfiguration : IEntityTypeConfiguration<OsmEntity>
{
    /// <summary>
    /// Configures the OsmEntity base type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<OsmEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Version)
            .IsRequired();

        builder.Property(e => e.Visible)
            .IsRequired();

        // Table per hierarchy (TPH) inheritance strategy
        builder.HasDiscriminator<string>("EntityType")
            .HasValue<OsmNode>("Node")
            .HasValue<OsmWay>("Way")
            .HasValue<OsmRelation>("Relation");

        // Optional properties
        builder.Property(e => e.ChangesetId);
        builder.Property(e => e.UserId);
        builder.Property(e => e.Timestamp);
    }
}
