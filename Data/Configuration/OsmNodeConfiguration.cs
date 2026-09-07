using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

public class OsmNodeConfiguration : IEntityTypeConfiguration<OsmNode>
{
    public void Configure(EntityTypeBuilder<OsmNode> builder)
    {
        // Use Table-Per-Concrete-Type (TPC) inheritance strategy
        builder.UseTpcMappingStrategy();

        // Primary key
        builder.HasKey(n => n.Id);

        // TPC: maps to separate table with all properties
        builder.ToTable("OsmNode");

        // Configure base properties (from IOsmEntity)
        builder.Property(n => n.Id)
            .ValueGeneratedNever(); // OSM IDs come from source data

        builder.Property(n => n.ChangesetId)
            .IsRequired(false);

        builder.Property(n => n.Version)
            .IsRequired();

        builder.Property(n => n.UserId)
            .IsRequired(false);

        builder.Property(n => n.Visible)
            .IsRequired();

        builder.Property(n => n.Timestamp)
            .IsRequired(false);

        // Node-specific properties
        builder.Property(n => n.Latitude)
            .IsRequired();

        builder.Property(n => n.Longitude)
            .IsRequired();

        // Indexes for spatial queries
        builder.HasIndex(n => new { n.Latitude, n.Longitude });
    }
}
