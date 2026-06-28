using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

/// <summary>
/// Entity type configuration for the <see cref="RelationMember"/> entity.
/// </summary>
public class RelationMemberConfiguration : IEntityTypeConfiguration<RelationMember>
{
    /// <summary>
    /// Configures the RelationMember entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<RelationMember> builder)
    {
        builder.HasKey(rm => rm.Id);

        builder.Property(rm => rm.Role)
            .HasMaxLength(255);

        builder.Property(rm => rm.SequenceNumber)
            .IsRequired();

        // Foreign key relationship to OsmRelation
        builder.HasOne(rm => rm.Relation)
            .WithMany(r => r.Members)
            .HasForeignKey(rm => rm.RelationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key relationship to OsmEntity (polymorphic - can be Node, Way, or Relation)
        builder.HasOne(rm => rm.Member)
            .WithMany()
            .HasForeignKey(rm => rm.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for common queries
        builder.HasIndex(rm => rm.RelationId);
        builder.HasIndex(rm => rm.MemberId);
        builder.HasIndex(rm => new { rm.RelationId, rm.SequenceNumber });
    }
}
