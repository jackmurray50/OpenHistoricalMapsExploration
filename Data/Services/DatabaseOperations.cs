using DataTypes.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;

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
        // With TPC, truncate in order (children first)
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"RelationMembers\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"WayNodes\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"OsmTags\" CASCADE");

        // Truncate entity tables
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"OsmNode\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"OsmWay\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"OsmRelation\" CASCADE");
    }

    /// <summary>
    /// Adds or updates a collection of OSM nodes (upsert).
    /// </summary>
    public async Task UpsertNodesAsync(IEnumerable<OsmNode> nodes, int batchSize = 1000)
    {
        var nodeList = nodes.ToList();
        if (nodeList.Count == 0)
            return;

        // Bulk insert nodes
        await BulkInsertNodesAsync(nodeList);
    }

    /// <summary>
    /// Adds or updates a collection of OSM ways (upsert).
    /// </summary>
    public async Task UpsertWaysAsync(IEnumerable<OsmWay> ways, int batchSize = 1000)
    {
        var wayList = ways.ToList();
        if (wayList.Count == 0)
            return;

        // Bulk insert ways
        await BulkInsertWaysAsync(wayList);
    }

    /// <summary>
    /// Adds or updates a collection of OSM relations (upsert).
    /// </summary>
    public async Task UpsertRelationsAsync(IEnumerable<OsmRelation> relations, int batchSize = 1000)
    {
        var relationList = relations.ToList();
        if (relationList.Count == 0)
            return;

        // Bulk insert relations
        await BulkInsertRelationsAsync(relationList);
    }

    private async Task BulkInsertNodesAsync(List<OsmNode> nodes)
    {
        if (nodes.Count == 0)
            return;

        // Process in smaller chunks to avoid connection timeouts
        const int chunkSize = 50_000;
        for (int i = 0; i < nodes.Count; i += chunkSize)
        {
            var chunk = nodes.Skip(i).Take(chunkSize).ToList();
            await BulkInsertNodesChunkAsync(chunk);
        }
    }

    private async Task BulkInsertNodesChunkAsync(List<OsmNode> nodes)
    {
        if (nodes.Count == 0)
            return;

        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();

        // Ensure connection is open
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        // Insert directly into OsmNode table with all properties (TPC)
        const string nodeSql = "COPY \"OsmNode\" (\"Id\", \"ChangesetId\", \"Version\", \"UserId\", \"Visible\", \"Timestamp\", \"Latitude\", \"Longitude\") FROM STDIN (FORMAT BINARY)";
        using (var writer = await connection.BeginBinaryImportAsync(nodeSql))
        {
            foreach (var node in nodes)
            {
                writer.StartRow();
                writer.Write(node.Id, NpgsqlDbType.Bigint);
                writer.Write(node.ChangesetId, NpgsqlDbType.Bigint);
                writer.Write(node.Version, NpgsqlDbType.Integer);
                writer.Write(node.UserId, NpgsqlDbType.Bigint);
                writer.Write(node.Visible, NpgsqlDbType.Boolean);
                writer.Write(node.Timestamp, NpgsqlDbType.TimestampTz);
                writer.Write(node.Latitude, NpgsqlDbType.Double);
                writer.Write(node.Longitude, NpgsqlDbType.Double);
            }
            await writer.CompleteAsync();
        }
    }

    private async Task BulkInsertWaysAsync(List<OsmWay> ways)
    {
        if (ways.Count == 0)
            return;

        // Process in smaller chunks to avoid connection timeouts
        const int chunkSize = 50_000;
        for (int i = 0; i < ways.Count; i += chunkSize)
        {
            var chunk = ways.Skip(i).Take(chunkSize).ToList();
            await BulkInsertWaysChunkAsync(chunk);
        }
    }

    private async Task BulkInsertWaysChunkAsync(List<OsmWay> ways)
    {
        if (ways.Count == 0)
            return;

        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        // Insert directly into OsmWay table with all properties (TPC)
        const string waySql = "COPY \"OsmWay\" (\"Id\", \"ChangesetId\", \"Version\", \"UserId\", \"Visible\", \"Timestamp\") FROM STDIN (FORMAT BINARY)";
        using (var writer = await connection.BeginBinaryImportAsync(waySql))
        {
            foreach (var way in ways)
            {
                writer.StartRow();
                writer.Write(way.Id, NpgsqlDbType.Bigint);
                writer.Write(way.ChangesetId, NpgsqlDbType.Bigint);
                writer.Write(way.Version, NpgsqlDbType.Integer);
                writer.Write(way.UserId, NpgsqlDbType.Bigint);
                writer.Write(way.Visible, NpgsqlDbType.Boolean);
                writer.Write(way.Timestamp, NpgsqlDbType.TimestampTz);
            }
            await writer.CompleteAsync();
        }
    }

    private async Task BulkInsertRelationsAsync(List<OsmRelation> relations)
    {
        if (relations.Count == 0)
            return;

        // Process in smaller chunks to avoid connection timeouts
        const int chunkSize = 50_000;
        for (int i = 0; i < relations.Count; i += chunkSize)
        {
            var chunk = relations.Skip(i).Take(chunkSize).ToList();
            await BulkInsertRelationsChunkAsync(chunk);
        }
    }

    private async Task BulkInsertRelationsChunkAsync(List<OsmRelation> relations)
    {
        if (relations.Count == 0)
            return;

        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        // Insert directly into OsmRelation table with all properties (TPC)
        const string relationSql = "COPY \"OsmRelation\" (\"Id\", \"ChangesetId\", \"Version\", \"UserId\", \"Visible\", \"Timestamp\") FROM STDIN (FORMAT BINARY)";
        using (var writer = await connection.BeginBinaryImportAsync(relationSql))
        {
            foreach (var relation in relations)
            {
                writer.StartRow();
                writer.Write(relation.Id, NpgsqlDbType.Bigint);
                writer.Write(relation.ChangesetId, NpgsqlDbType.Bigint);
                writer.Write(relation.Version, NpgsqlDbType.Integer);
                writer.Write(relation.UserId, NpgsqlDbType.Bigint);
                writer.Write(relation.Visible, NpgsqlDbType.Boolean);
                writer.Write(relation.Timestamp, NpgsqlDbType.TimestampTz);
            }
            await writer.CompleteAsync();
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
        if (tagList.Count == 0)
            return;

        const string sql = "COPY \"OsmTags\" (\"Id\", \"Key\", \"Value\", \"EntityId\", \"EntityType\") FROM STDIN (FORMAT BINARY)";
        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using (var writer = await connection.BeginBinaryImportAsync(sql))
        {
            foreach (var tag in tagList)
            {
                writer.StartRow();
                writer.Write(tag.Id, NpgsqlDbType.Bigint);
                writer.Write(tag.Key, NpgsqlDbType.Varchar);
                writer.Write(tag.Value, NpgsqlDbType.Varchar);
                writer.Write(tag.EntityId, NpgsqlDbType.Bigint);
                writer.Write(tag.EntityType, NpgsqlDbType.Varchar);
            }
            await writer.CompleteAsync();
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
        if (wayNodeList.Count == 0)
            return;

        const string sql = "COPY \"WayNodes\" (\"Id\", \"WayId\", \"NodeId\", \"SequenceNumber\") FROM STDIN (FORMAT BINARY)";
        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
        
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using (var writer = await connection.BeginBinaryImportAsync(sql))
        {
            foreach (var wayNode in wayNodeList)
            {
                writer.StartRow();
                writer.Write(wayNode.Id, NpgsqlDbType.Bigint);
                writer.Write(wayNode.WayId, NpgsqlDbType.Bigint);
                writer.Write(wayNode.NodeId, NpgsqlDbType.Bigint);
                writer.Write(wayNode.SequenceNumber, NpgsqlDbType.Integer);
            }
            await writer.CompleteAsync();
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
        if (relationMemberList.Count == 0)
            return;

        const string sql = "COPY \"RelationMembers\" (\"Id\", \"RelationId\", \"MemberId\", \"Role\", \"SequenceNumber\") FROM STDIN (FORMAT BINARY)";
        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
        
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using (var writer = await connection.BeginBinaryImportAsync(sql))
        {
            foreach (var member in relationMemberList)
            {
                writer.StartRow();
                writer.Write(member.Id, NpgsqlDbType.Bigint);
                writer.Write(member.RelationId, NpgsqlDbType.Bigint);
                writer.Write(member.MemberId, NpgsqlDbType.Bigint);
                writer.Write(member.Role, NpgsqlDbType.Varchar);
                writer.Write(member.SequenceNumber, NpgsqlDbType.Integer);
            }
            await writer.CompleteAsync();
        }
    }

    /// <summary>
    /// Gets the count of each entity type in the database.
    /// </summary>
    /// <returns>A tuple with counts of (entities, tags, wayNodes, relationMembers).</returns>
    public async Task<(long EntityCount, long TagCount, long WayNodeCount, long RelationMemberCount)> GetCountsAsync()
    {
        var nodeCount = _context.Nodes.Count();
        var wayCount = _context.Ways.Count();
        var relationCount = _context.Relations.Count();
        var entityCount = nodeCount + wayCount + relationCount;
        var tagCount = _context.OsmTags.Count();
        var wayNodeCount = _context.WayNodes.Count();
        var relationMemberCount = _context.RelationMembers.Count();

        return await Task.FromResult((entityCount, tagCount, wayNodeCount, relationMemberCount));
    }
}
