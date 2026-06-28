using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

/// <summary>
/// Database context for the Historical Map application. Provides access to OpenStreetMap entities
/// stored in the database.
/// </summary>
public class HistoricalMapDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoricalMapDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public HistoricalMapDbContext(DbContextOptions<HistoricalMapDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet for OSM nodes.
    /// </summary>
    public DbSet<OsmNode> Nodes { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for OSM ways.
    /// </summary>
    public DbSet<OsmWay> Ways { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for OSM relations.
    /// </summary>
    public DbSet<OsmRelation> Relations { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for tags.
    /// </summary>
    public DbSet<OsmTag> OsmTags { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for way nodes.
    /// </summary>
    public DbSet<WayNode> WayNodes { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for relation members.
    /// </summary>
    public DbSet<RelationMember> RelationMembers { get; set; }

    /// <summary>
    /// Configures the model and relationships using Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the Configuration folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HistoricalMapDbContext).Assembly);
    }
}
