using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

/// <summary>
/// Entity type configuration for the <see cref="OsmNode"/> entity.
/// </summary>
public class OsmNodeConfiguration : IEntityTypeConfiguration<OsmNode>
{
    /// <summary>
    /// Configures the OsmNode entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<OsmNode> builder)
    {
        builder.Property(n => n.Longitude)
            .IsRequired();

        builder.Property(n => n.Latitude)
            .IsRequired();

        // Indexes for geospatial queries
        builder.HasIndex(n => n.Latitude);
        builder.HasIndex(n => n.Longitude);
        builder.HasIndex(n => new { n.Latitude, n.Longitude });
    }
}
