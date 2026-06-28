using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

/// <summary>
/// Entity type configuration for the <see cref="WayNode"/> entity.
/// </summary>
public class WayNodeConfiguration : IEntityTypeConfiguration<WayNode>
{
    /// <summary>
    /// Configures the WayNode entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<WayNode> builder)
    {
        builder.HasKey(wn => wn.Id);

        builder.Property(wn => wn.SequenceNumber)
            .IsRequired();

        // Foreign key relationship to OsmWay
        builder.HasOne(wn => wn.Way)
            .WithMany(w => w.WayNodes)
            .HasForeignKey(wn => wn.WayId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key relationship to OsmNode
        builder.HasOne(wn => wn.Node)
            .WithMany(n => n.WayNodes)
            .HasForeignKey(wn => wn.NodeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for common queries
        builder.HasIndex(wn => wn.WayId);
        builder.HasIndex(wn => wn.NodeId);
        builder.HasIndex(wn => new { wn.WayId, wn.SequenceNumber });
    }
}
