using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

/// <summary>
/// Entity type configuration for the <see cref="OsmRelation"/> entity.
/// </summary>
public class OsmRelationConfiguration : IEntityTypeConfiguration<OsmRelation>
{
    /// <summary>
    /// Configures the OsmRelation entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<OsmRelation> builder)
    {
        // Configuration delegated to OsmEntityConfiguration
    }
}
