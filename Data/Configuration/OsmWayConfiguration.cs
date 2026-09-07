using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

public class OsmWayConfiguration : IEntityTypeConfiguration<OsmWay>
{
    public void Configure(EntityTypeBuilder<OsmWay> builder)
    {
        // Use Table-Per-Concrete-Type (TPC) inheritance strategy
        builder.UseTpcMappingStrategy();

        // Primary key
        builder.HasKey(w => w.Id);

        // TPC: maps to separate table with all properties
        builder.ToTable("OsmWay");

        // Configure base properties (from IOsmEntity)
        builder.Property(w => w.Id)
            .ValueGeneratedNever(); // OSM IDs come from source data

        builder.Property(w => w.ChangesetId)
            .IsRequired(false);

        builder.Property(w => w.Version)
            .IsRequired();

        builder.Property(w => w.UserId)
            .IsRequired(false);

        builder.Property(w => w.Visible)
            .IsRequired();

        builder.Property(w => w.Timestamp)
            .IsRequired(false);
    }
}
