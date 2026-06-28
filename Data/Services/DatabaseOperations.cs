using DataTypes.Entities;

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
        _context.RelationMembers.RemoveRange(_context.RelationMembers);
        _context.WayNodes.RemoveRange(_context.WayNodes);
        _context.OsmTags.RemoveRange(_context.OsmTags);

        // Nodes, Ways, Relations are in the same table (TPH), so remove them via the base Entities query
        _context.ChangeTracker.DetectChanges();
        var entitiesToRemove = _context.ChangeTracker.Entries<OsmEntity>()
            .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added || 
                       e.State == Microsoft.EntityFrameworkCore.EntityState.Unchanged ||
                       e.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
            .Select(e => e.Entity)
            .ToList();

        if (entitiesToRemove.Any())
        {
            _context.RemoveRange(entitiesToRemove);
        }

        await _context.SaveChangesAsync();
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
        var batches = entityList.Chunk(batchSize);

        foreach (var batch in batches)
        {
            foreach (var entity in batch)
            {
                var existing = await _context.Set<OsmEntity>().FindAsync(entity.Id);
                if (existing != null)
                {
                    // Update existing entity
                    _context.Entry(existing).CurrentValues.SetValues(entity);
                    existing.Tags.Clear();
                    foreach (var tag in entity.Tags)
                    {
                        existing.Tags.Add(tag);
                    }
                }
                else
                {
                    // Add new entity
                    _context.Add(entity);
                }
            }

            await _context.SaveChangesAsync();
        }
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
        var batches = tagList.Chunk(batchSize);

        foreach (var batch in batches)
        {
            foreach (var tag in batch)
            {
                var existing = await _context.OsmTags.FindAsync(tag.Id);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(tag);
                }
                else
                {
                    _context.OsmTags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();
        }
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
        var batches = wayNodeList.Chunk(batchSize);

        foreach (var batch in batches)
        {
            foreach (var wayNode in batch)
            {
                var existing = await _context.WayNodes.FindAsync(wayNode.Id);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(wayNode);
                }
                else
                {
                    _context.WayNodes.Add(wayNode);
                }
            }

            await _context.SaveChangesAsync();
        }
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
        var batches = relationMemberList.Chunk(batchSize);

        foreach (var batch in batches)
        {
            foreach (var relationMember in batch)
            {
                var existing = await _context.RelationMembers.FindAsync(relationMember.Id);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(relationMember);
                }
                else
                {
                    _context.RelationMembers.Add(relationMember);
                }
            }

            await _context.SaveChangesAsync();
        }
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
