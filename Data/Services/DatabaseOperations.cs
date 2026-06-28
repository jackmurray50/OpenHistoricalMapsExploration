using DataTypes.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

/// <summary>
/// Service for database operations during data import (update vs overwrite modes).
/// </summary>
public class DatabaseOperations
{
    private readonly HistoricalMapDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseOperations"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DatabaseOperations(HistoricalMapDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Clears all OSM data from the database (nodes, ways, relations, tags, way nodes, relation members).
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ClearAllDataAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"RelationMembers\" DISABLE TRIGGER ALL");
        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"WayNodes\" DISABLE TRIGGER ALL");
        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmTags\" DISABLE TRIGGER ALL");

        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"RelationMembers\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"WayNodes\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"OsmTags\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"OsmEntity\" CASCADE");

        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmTags\" ENABLE TRIGGER ALL");
        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"WayNodes\" ENABLE TRIGGER ALL");
        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"RelationMembers\" ENABLE TRIGGER ALL");
    }

    /// <summary>
    /// Adds or updates a collection of OSM entities (upsert).
    /// </summary>
    /// <param name="entities">The entities to add or update.</param>
    /// <param name="batchSize">The number of entities to save per batch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpsertEntitiesAsync(IEnumerable<OsmEntity> entities, int batchSize = 1000)
    {
        var entityList = entities.ToList();
        if (entityList.Count == 0)
            return;

        // Separate entities by type
        var nodes = entityList.OfType<OsmNode>().ToList();
        var ways = entityList.OfType<OsmWay>().ToList();
        var relations = entityList.OfType<OsmRelation>().ToList();

        // Insert all nodes in one go
        if (nodes.Count > 0)
        {
            _context.Nodes.AddRange(nodes);
        }

        // Insert all ways in one go
        if (ways.Count > 0)
        {
            _context.Ways.AddRange(ways);
        }

        // Insert all relations in one go
        if (relations.Count > 0)
        {
            _context.Relations.AddRange(relations);
        }

        // Single SaveChangesAsync for the entire 5M batch
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    /// <summary>
    /// Adds or updates tags for entities (upsert).
    /// </summary>
    /// <param name="tags">The tags to add or update.</param>
    /// <param name="batchSize">The number of tags to save per batch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpsertTagsAsync(IEnumerable<OsmTag> tags, int batchSize = 2000)
    {
        var tagList = tags.ToList();
        if (tagList.Count == 0)
            return;

        await _context.BulkInsertOrUpdateAsync(tagList, new BulkConfig
        {
            BatchSize = batchSize,
            CalculateStats = false
        });
    }

    /// <summary>
    /// Adds or updates way nodes (upsert).
    /// </summary>
    /// <param name="wayNodes">The way nodes to add or update.</param>
    /// <param name="batchSize">The number of way nodes to save per batch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpsertWayNodesAsync(IEnumerable<WayNode> wayNodes, int batchSize = 5000)
    {
        var wayNodeList = wayNodes.ToList();
        if (wayNodeList.Count == 0)
            return;

        await _context.BulkInsertOrUpdateAsync(wayNodeList, new BulkConfig
        {
            BatchSize = batchSize,
            CalculateStats = false
        });
    }

    /// <summary>
    /// Adds or updates relation members (upsert).
    /// </summary>
    /// <param name="relationMembers">The relation members to add or update.</param>
    /// <param name="batchSize">The number of relation members to save per batch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpsertRelationMembersAsync(IEnumerable<RelationMember> relationMembers, int batchSize = 5000)
    {
        var relationMemberList = relationMembers.ToList();
        if (relationMemberList.Count == 0)
            return;

        await _context.BulkInsertOrUpdateAsync(relationMemberList, new BulkConfig
        {
            BatchSize = batchSize,
            CalculateStats = false
        });
    }

    /// <summary>
    /// Gets the count of each entity type in the database.
    /// </summary>
    /// <returns>A tuple with counts of (entities, tags, wayNodes, relationMembers).</returns>
    public async Task<(long EntityCount, long TagCount, long WayNodeCount, long RelationMemberCount)> GetCountsAsync()
    {
        var entityCount = _context.Set<OsmEntity>().Count();
        var tagCount = _context.OsmTags.Count();
        var wayNodeCount = _context.WayNodes.Count();
        var relationMemberCount = _context.RelationMembers.Count();

        return await Task.FromResult((entityCount, tagCount, wayNodeCount, relationMemberCount));
    }
}
