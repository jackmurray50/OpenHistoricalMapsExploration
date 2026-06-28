using DataTypes.Entities;
using OsmSharp;
using OsmSharp.Streams;

namespace Data.Services;

/// <summary>
/// Service for importing OSM data from PBF files using OsmSharp.
/// Converts OsmSharp elements to DataTypes entities.
/// </summary>
public class OsmDataImporter
{
    private long _tagId = 1;
    private long _wayNodeId = 1;
    private long _relationMemberId = 1;

    /// <summary>
    /// Reads and imports OSM data from a PBF file.
    /// </summary>
    /// <param name="pbfFilePath">Path to the PBF file.</param>
    /// <returns>
    /// A tuple containing lists of (entities, tags, wayNodes, relationMembers) extracted from the PBF file.
    /// </returns>
    /// <exception cref="FileNotFoundException">Thrown if the PBF file does not exist.</exception>
    public (List<OsmEntity> Entities, List<OsmTag> Tags, List<WayNode> WayNodes, List<DataTypes.Entities.RelationMember> RelationMembers) 
        ImportFromPbf(string pbfFilePath)
    {
        if (!File.Exists(pbfFilePath))
        {
            throw new FileNotFoundException($"PBF file not found: {pbfFilePath}");
        }

        var entities = new List<OsmEntity>();
        var tags = new List<OsmTag>();
        var wayNodes = new List<WayNode>();
        var relationMembers = new List<DataTypes.Entities.RelationMember>();

        using (var fileStream = File.OpenRead(pbfFilePath))
        {
            var source = new OsmSharp.Streams.PBFOsmStreamSource(fileStream);

            foreach (var osmObject in source)
            {
                if (osmObject == null)
                    continue;

                switch (osmObject.Type)
                {
                    case OsmGeoType.Node:
                        var osmNode = ConvertNode((Node)osmObject);
                        entities.Add(osmNode);
                        tags.AddRange(ConvertTags(osmObject.Tags, osmNode.Id));
                        break;

                    case OsmGeoType.Way:
                        var osmWay = ConvertWay((Way)osmObject);
                        entities.Add(osmWay);
                        tags.AddRange(ConvertTags(osmObject.Tags, osmWay.Id));
                        wayNodes.AddRange(ConvertWayNodes((Way)osmObject, osmWay.Id));
                        break;

                    case OsmGeoType.Relation:
                        var osmRelation = ConvertRelation((Relation)osmObject);
                        entities.Add(osmRelation);
                        tags.AddRange(ConvertTags(osmObject.Tags, osmRelation.Id));
                        relationMembers.AddRange(ConvertRelationMembers((Relation)osmObject, osmRelation.Id));
                        break;
                }
            }
        }

        return (entities, tags, wayNodes, relationMembers);
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
            Timestamp = node.TimeStamp,
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
            Timestamp = way.TimeStamp,
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
            Timestamp = relation.TimeStamp,
            Version = relation.Version ?? 1,
        };
    }

    /// <summary>
    /// Converts OsmSharp tags to OsmTag entities.
    /// </summary>
    private List<OsmTag> ConvertTags(OsmSharp.Tags.TagsCollectionBase osmTags, long entityId)
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
            relationMembers.Add(new DataTypes.Entities.RelationMember
            {
                Id = _relationMemberId++,
                RelationId = relationId,
                MemberId = member.Id,
                Role = member.Role,
                SequenceNumber = i,
            });
        }

        return relationMembers;
    }
}
