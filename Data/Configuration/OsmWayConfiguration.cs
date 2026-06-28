using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

/// <summary>
/// Entity type configuration for the <see cref="OsmWay"/> entity.
/// </summary>
public class OsmWayConfiguration : IEntityTypeConfiguration<OsmWay>
{
    /// <summary>
    /// Configures the OsmWay entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<OsmWay> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Version)
            .IsRequired();

        builder.Property(w => w.Visible)
            .IsRequired();
    }
}
