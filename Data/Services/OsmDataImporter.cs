using DataTypes.Entities;
using OsmSharp;
using OsmSharp.Streams;
using System.Collections.Concurrent;

namespace Data.Services;

/// <summary>
/// Service for importing OSM data from PBF files using OsmSharp with streaming writes.
/// Converts OsmSharp elements to DataTypes entities and streams them to the database in chunks.
/// </summary>
public class OsmDataImporter
{
    private long _tagId = 1;
    private long _wayNodeId = 1;
    private long _relationMemberId = 1;

    // Streaming batch configuration
    private const int FLUSH_SIZE = 15_000_000; // Flush to database every X objects
    private const int PARALLEL_TASKS = 1; // Number of concurrent database operations (serialized to prevent connection busy errors)

    /// <summary>
    /// Callback for progress reporting during streaming import.
    /// </summary>
    public delegate void ImportProgressCallback(long itemsProcessed, long nodesInBatch, long waysInBatch, long relationsInBatch, long tagsInBatch);

    /// <summary>
    /// Event raised during import to report progress.
    /// </summary>
    public event ImportProgressCallback? OnProgress;

    /// <summary>
    /// Reads and imports OSM data from a PBF file with streaming writes.
    /// Batches are written to the database every FLUSH_SIZE items to manage memory.
    /// </summary>
    /// <param name="pbfFilePath">Path to the PBF file.</param>
    /// <param name="databaseOps">The database operations service for writing batches.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the PBF file does not exist.</exception>
     public async Task ImportFromPbfStreamingAsync(string pbfFilePath, DatabaseOperations databaseOps)
    {
        if (!File.Exists(pbfFilePath))
        {
            throw new FileNotFoundException($"PBF file not found: {pbfFilePath}");
        }

        var nodes = new List<OsmNode>();
        var ways = new List<OsmWay>();
        var relations = new List<OsmRelation>();
        var tags = new List<OsmTag>();
        var wayNodes = new List<WayNode>();
        var relationMembers = new List<DataTypes.Entities.RelationMember>();

        var parseStopwatch = System.Diagnostics.Stopwatch.StartNew();
        long itemsProcessed = 0;
        var flushTasks = new ConcurrentQueue<Task>();

        using (var fileStream = File.OpenRead(pbfFilePath))
        {
            var source = new OsmSharp.Streams.PBFOsmStreamSource(fileStream);

            foreach (var osmObject in source)
            {
                if (osmObject == null)
                    continue;

                itemsProcessed++;

                // Parse the OSM object and add to appropriate lists
                switch (osmObject.Type)
                {
                    case OsmGeoType.Node:
                        var osmNode = ConvertNode((Node)osmObject);
                        nodes.Add(osmNode);
                        tags.AddRange(ConvertTags(osmObject.Tags, osmNode.Id, "Node"));
                        break;

                    case OsmGeoType.Way:
                        var osmWay = ConvertWay((Way)osmObject);
                        ways.Add(osmWay);
                        tags.AddRange(ConvertTags(osmObject.Tags, osmWay.Id, "Way"));
                        wayNodes.AddRange(ConvertWayNodes((Way)osmObject, osmWay.Id));
                        break;

                    case OsmGeoType.Relation:
                        var osmRelation = ConvertRelation((Relation)osmObject);
                        relations.Add(osmRelation);
                        tags.AddRange(ConvertTags(osmObject.Tags, osmRelation.Id, "Relation"));
                        relationMembers.AddRange(ConvertRelationMembers((Relation)osmObject, osmRelation.Id));
                        break;
                }

                // Flush to database when batch reaches FLUSH_SIZE
                if (itemsProcessed % FLUSH_SIZE == 0)
                {
                    parseStopwatch.Stop();
                    var parseTime = parseStopwatch.ElapsedMilliseconds;
                    var itemsPerSecond = (itemsProcessed / (parseTime / 1000.0));
                    Console.WriteLine($"Parse speed: {itemsPerSecond:N0} items/sec ({parseTime}ms for {itemsProcessed:N0} items)");
                    parseStopwatch.Restart();

                    // Report progress BEFORE clearing
                    OnProgress?.Invoke(itemsProcessed, nodes.Count, ways.Count, relations.Count, tags.Count);

                    // Create a copy of the current batch for async processing
                    var batchNodes = new List<OsmNode>(nodes);
                    var batchWays = new List<OsmWay>(ways);
                    var batchRelations = new List<OsmRelation>(relations);
                    var batchTags = new List<OsmTag>(tags);
                    var batchWayNodes = new List<WayNode>(wayNodes);
                    var batchRelationMembers = new List<DataTypes.Entities.RelationMember>(relationMembers);

                    // Queue the flush task
                    var flushTask = FlushBatchAsync(databaseOps, batchNodes, batchWays, batchRelations, batchTags, batchWayNodes, batchRelationMembers);
                    flushTasks.Enqueue(flushTask);

                    // Wait if we have too many pending tasks (limit concurrency)
                    while (flushTasks.Count >= PARALLEL_TASKS)
                    {
                        if (flushTasks.TryDequeue(out var task))
                        {
                            await task;
                        }
                    }

                    // Clear batches for next iteration
                    nodes.Clear();
                    ways.Clear();
                    relations.Clear();
                    tags.Clear();
                    wayNodes.Clear();
                    relationMembers.Clear();
                }
            }

            // Flush any remaining items in the final batch
            if (nodes.Count > 0 || ways.Count > 0 || relations.Count > 0 || tags.Count > 0 || wayNodes.Count > 0 || relationMembers.Count > 0)
            {
                var batchNodes = new List<OsmNode>(nodes);
                var batchWays = new List<OsmWay>(ways);
                var batchRelations = new List<OsmRelation>(relations);
                var batchTags = new List<OsmTag>(tags);
                var batchWayNodes = new List<WayNode>(wayNodes);
                var batchRelationMembers = new List<DataTypes.Entities.RelationMember>(relationMembers);

                var flushTask = FlushBatchAsync(databaseOps, batchNodes, batchWays, batchRelations, batchTags, batchWayNodes, batchRelationMembers);
                flushTasks.Enqueue(flushTask);
            }

            // Wait for all remaining flush tasks to complete
            Console.WriteLine("Waiting for all batch writes to complete...");
            while (flushTasks.TryDequeue(out var task))
            {
                await task;
            }

            OnProgress?.Invoke(itemsProcessed, 0, 0, 0, 0);
        }
    }

    /// <summary>
    /// Flushes the current batch to the database.
    /// </summary>
    private async Task FlushBatchAsync(
        DatabaseOperations databaseOps,
        List<OsmNode> nodes,
        List<OsmWay> ways,
        List<OsmRelation> relations,
        List<OsmTag> tags,
        List<WayNode> wayNodes,
        List<DataTypes.Entities.RelationMember> relationMembers)
    {
        if (nodes.Count > 0)
            await databaseOps.UpsertNodesAsync(nodes, batchSize: 100_000);

        if (ways.Count > 0)
            await databaseOps.UpsertWaysAsync(ways, batchSize: 100_000);

        if (relations.Count > 0)
            await databaseOps.UpsertRelationsAsync(relations, batchSize: 100_000);

        if (tags.Count > 0)
            await databaseOps.UpsertTagsAsync(tags, batchSize: 100_000);

        if (wayNodes.Count > 0)
            await databaseOps.UpsertWayNodesAsync(wayNodes, batchSize: 100_000);

        if (relationMembers.Count > 0)
            await databaseOps.UpsertRelationMembersAsync(relationMembers, batchSize: 100_000);
    }

    /// <summary>
    /// Converts an OsmSharp Node to an OsmNode entity.
    /// </summary>
    private OsmNode ConvertNode(Node node)
    {
        return new OsmNode
        {
            Id = node.Id!.Value,
            Latitude = node.Latitude ?? 0,
            Longitude = node.Longitude ?? 0,
            ChangesetId = node.ChangeSetId,
            UserId = node.UserId,
            Visible = node.Visible ?? true,
            Timestamp = ConvertToUtcDateTime(node.TimeStamp),
            Version = node.Version ?? 1,
        };
    }

    /// <summary>
    /// Converts an OsmSharp Way to an OsmWay entity.
    /// </summary>
    private OsmWay ConvertWay(Way way)
    {
        return new OsmWay
        {
            Id = way.Id!.Value,
            ChangesetId = way.ChangeSetId,
            UserId = way.UserId,
            Visible = way.Visible ?? true,
            Timestamp = ConvertToUtcDateTime(way.TimeStamp),
            Version = way.Version ?? 1,
        };
    }

    /// <summary>
    /// Converts an OsmSharp Relation to an OsmRelation entity.
    /// </summary>
    private OsmRelation ConvertRelation(Relation relation)
    {
        return new OsmRelation
        {
            Id = relation.Id!.Value,
            ChangesetId = relation.ChangeSetId,
            UserId = relation.UserId,
            Visible = relation.Visible ?? true,
            Timestamp = ConvertToUtcDateTime(relation.TimeStamp),
            Version = relation.Version ?? 1,
        };
    }

    /// <summary>
    /// Converts OsmSharp tags to OsmTag entities.
    /// </summary>
    private List<OsmTag> ConvertTags(OsmSharp.Tags.TagsCollectionBase osmTags, long entityId, string entityType)
    {
        var tags = new List<OsmTag>();
        if (osmTags == null || osmTags.Count == 0)
            return tags;

        foreach (var tag in osmTags)
        {
            tags.Add(new OsmTag
            {
                Id = _tagId++,
                Key = tag.Key,
                Value = tag.Value,
                EntityId = entityId,
                EntityType = entityType,
            });
        }

        return tags;
    }

    /// <summary>
    /// Converts OsmSharp way nodes to WayNode entities.
    /// </summary>
    private List<WayNode> ConvertWayNodes(Way way, long wayId)
    {
        var wayNodes = new List<WayNode>();
        if (way.Nodes == null || way.Nodes.Length == 0)
            return wayNodes;

        for (int i = 0; i < way.Nodes.Length; i++)
        {
            wayNodes.Add(new WayNode
            {
                Id = _wayNodeId++,
                WayId = wayId,
                NodeId = way.Nodes[i],
                SequenceNumber = i,
            });
        }

        return wayNodes;
    }

    /// <summary>
    /// Converts OsmSharp relation members to DataTypes.Entities.RelationMember entities.
    /// </summary>
    private List<DataTypes.Entities.RelationMember> ConvertRelationMembers(Relation relation, long relationId)
    {
        var relationMembers = new List<DataTypes.Entities.RelationMember>();
        if (relation.Members == null || relation.Members.Length == 0)
            return relationMembers;

        for (int i = 0; i < relation.Members.Length; i++)
        {
            var member = relation.Members[i];
            var memberType = member.Type switch
            {
                OsmGeoType.Node => "Node",
                OsmGeoType.Way => "Way",
                OsmGeoType.Relation => "Relation",
                _ => "Unknown"
            };
            relationMembers.Add(new DataTypes.Entities.RelationMember
            {
                Id = _relationMemberId++,
                RelationId = relationId,
                MemberId = member.Id,
                MemberType = memberType,
                Role = member.Role,
                SequenceNumber = i,
            });
        }

        return relationMembers;
    }
    // Add this new helper method before the closing brace of the class
    private DateTime? ConvertToUtcDateTime(DateTime? dateTime)
    {
        if (dateTime == null)
            return null;

        var dt = dateTime.Value;

        if (dt.Kind == DateTimeKind.Utc)
            return dt;

        if (dt.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        return dt.ToUniversalTime();
    }
}
