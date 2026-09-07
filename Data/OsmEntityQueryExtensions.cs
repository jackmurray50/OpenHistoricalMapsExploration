using DataTypes.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

/// <summary>
/// Extension methods for querying OSM entities across all types (nodes, ways, relations).
/// Since TPC inheritance doesn't support IQueryable<IOsmEntity>, these methods provide
/// polymorphic query capabilities through union operations.
/// </summary>
public static class OsmEntityQueryExtensions
{
    /// <summary>
    /// Gets all OSM entities (nodes, ways, and relations) as IOsmEntity instances.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>An enumerable of all OSM entities.</returns>
    public static async Task<IEnumerable<IOsmEntity>> GetAllEntitiesAsync(this HistoricalMapDbContext context)
    {
        var nodes = await context.Nodes.ToListAsync();
        var ways = await context.Ways.ToListAsync();
        var relations = await context.Relations.ToListAsync();

        var entities = new List<IOsmEntity>();
        entities.AddRange(nodes);
        entities.AddRange(ways);
        entities.AddRange(relations);

        return entities;
    }

    /// <summary>
    /// Finds a single OSM entity by ID, checking all entity types.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="id">The entity ID to find.</param>
    /// <returns>The entity if found; otherwise null.</returns>
    public static async Task<IOsmEntity?> FindEntityByIdAsync(this HistoricalMapDbContext context, long id)
    {
        var node = await context.Nodes.FirstOrDefaultAsync(n => n.Id == id);
        if (node != null)
            return node;

        var way = await context.Ways.FirstOrDefaultAsync(w => w.Id == id);
        if (way != null)
            return way;

        var relation = await context.Relations.FirstOrDefaultAsync(r => r.Id == id);
        if (relation != null)
            return relation;

        return null;
    }

    /// <summary>
    /// Finds all OSM entities with a specific tag key.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="tagKey">The tag key to search for.</param>
    /// <returns>An enumerable of entities with the specified tag.</returns>
    public static async Task<IEnumerable<IOsmEntity>> FindEntitiesByTagKeyAsync(this HistoricalMapDbContext context, string tagKey)
    {
        var entityIds = await context.OsmTags
            .Where(t => t.Key == tagKey)
            .Select(t => new { t.EntityId, t.EntityType })
            .Distinct()
            .ToListAsync();

        var entities = new List<IOsmEntity>();

        // Query nodes
        var nodeIds = entityIds.Where(e => e.EntityType == "Node").Select(e => e.EntityId.Value).ToList();
        if (nodeIds.Any())
        {
            var nodes = await context.Nodes.Where(n => nodeIds.Contains(n.Id)).ToListAsync();
            entities.AddRange(nodes);
        }

        // Query ways
        var wayIds = entityIds.Where(e => e.EntityType == "Way").Select(e => e.EntityId.Value).ToList();
        if (wayIds.Any())
        {
            var ways = await context.Ways.Where(w => wayIds.Contains(w.Id)).ToListAsync();
            entities.AddRange(ways);
        }

        // Query relations
        var relationIds = entityIds.Where(e => e.EntityType == "Relation").Select(e => e.EntityId.Value).ToList();
        if (relationIds.Any())
        {
            var relations = await context.Relations.Where(r => relationIds.Contains(r.Id)).ToListAsync();
            entities.AddRange(relations);
        }

        return entities;
    }

    /// <summary>
    /// Finds all OSM entities with a specific tag key-value pair.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="tagKey">The tag key.</param>
    /// <param name="tagValue">The tag value.</param>
    /// <returns>An enumerable of entities with the specified tag.</returns>
    public static async Task<IEnumerable<IOsmEntity>> FindEntitiesByTagAsync(this HistoricalMapDbContext context, string tagKey, string tagValue)
    {
        var entityIds = await context.OsmTags
            .Where(t => t.Key == tagKey && t.Value == tagValue)
            .Select(t => new { t.EntityId, t.EntityType })
            .Distinct()
            .ToListAsync();

        var entities = new List<IOsmEntity>();

        // Query nodes
        var nodeIds = entityIds.Where(e => e.EntityType == "Node").Select(e => e.EntityId.Value).ToList();
        if (nodeIds.Any())
        {
            var nodes = await context.Nodes.Where(n => nodeIds.Contains(n.Id)).ToListAsync();
            entities.AddRange(nodes);
        }

        // Query ways
        var wayIds = entityIds.Where(e => e.EntityType == "Way").Select(e => e.EntityId.Value).ToList();
        if (wayIds.Any())
        {
            var ways = await context.Ways.Where(w => wayIds.Contains(w.Id)).ToListAsync();
            entities.AddRange(ways);
        }

        // Query relations
        var relationIds = entityIds.Where(e => e.EntityType == "Relation").Select(e => e.EntityId.Value).ToList();
        if (relationIds.Any())
        {
            var relations = await context.Relations.Where(r => relationIds.Contains(r.Id)).ToListAsync();
            entities.AddRange(relations);
        }

        return entities;
    }

    /// <summary>
    /// Counts all OSM entities across all types.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>The total count of entities.</returns>
    public static async Task<long> CountAllEntitiesAsync(this HistoricalMapDbContext context)
    {
        var nodeCount = await context.Nodes.CountAsync();
        var wayCount = await context.Ways.CountAsync();
        var relationCount = await context.Relations.CountAsync();

        return nodeCount + wayCount + relationCount;
    }
}
