namespace DataTypes.Entities;

/// <summary>
/// Represents a member of a relation in OpenStreetMap (OSM) data. A relation member can be a node, way, or another relation that is part of a relation, which is a group of elements that define a complex feature or relationship. Each relation member has an associated role and sequence number that defines its position within the relation.
/// </summary>
public class RelationMember
{
    /// <summary>
    /// Gets or sets the unique identifier for the relation member.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated OSM relation. This property indicates which relation the member belongs to.
    /// </summary>
    public long RelationId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated OSM entity (node, way, or relation). This property indicates which entity is a member of the relation.
    /// </summary>
    public long MemberId { get; set; }

    /// <summary>
    /// Gets the type of the member entity (Node, Way, or Relation).
    /// </summary>
    public string MemberType
    {
        get => this.Member switch
        {
            OsmNode => "Node",
            OsmWay => "Way",
            OsmRelation => "Relation",
            _ => this.Member.GetType().Name,
        };
    }

    /// <summary>
    /// Gets or sets the role of the member within the relation. The role defines the purpose or function of the member in the context of the relation, such as "outer", "inner", "stop", etc. This property is optional and can be null if no specific role is assigned.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the sequence number of the member within the relation. The sequence number defines the order of the member in the relation.
    /// </summary>
    public int SequenceNumber { get; set; }

    // Navigation properties

    /// <summary>
    /// Gets or sets the navigation property for the associated OSM relation. This property allows access to the relation that the member belongs to.
    /// </summary>
    public OsmRelation Relation { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property for the associated OSM entity (node, way, or relation). This property allows access to the member entity that is part of the relation.
    /// </summary>
    public OsmEntity Member { get; set; } = null!;
}
