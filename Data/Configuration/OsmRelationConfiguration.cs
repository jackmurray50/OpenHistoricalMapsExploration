using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

public class OsmRelationConfiguration : IEntityTypeConfiguration<OsmRelation>
{
    public void Configure(EntityTypeBuilder<OsmRelation> builder)
    {
        // Use Table-Per-Concrete-Type (TPC) inheritance strategy
        builder.UseTpcMappingStrategy();

        // Primary key
        builder.HasKey(r => r.Id);

        // TPC: maps to separate table with all properties
        builder.ToTable("OsmRelation");

        // Configure base properties (from IOsmEntity)
        builder.Property(r => r.Id)
            .ValueGeneratedNever(); // OSM IDs come from source data

        builder.Property(r => r.ChangesetId)
            .IsRequired(false);

        builder.Property(r => r.Version)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired(false);

        builder.Property(r => r.Visible)
            .IsRequired();

        builder.Property(r => r.Timestamp)
            .IsRequired(false);
    }
}
