namespace DataTypes.Entities;

/// <summary>
/// Represents a node in OpenStreetMap (OSM) data. A node is a single point defined by its latitude and longitude coordinates. Nodes can be part of ways and relations, and they can have associated tags that provide additional information about the node.
/// </summary>
public class OsmNode : IOsmEntity
{
    // IOsmEntity properties

    /// <summary>
    /// Gets or sets the unique identifier for the OSM entity.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the Changeset ID of the OSM entity. A changeset represents a group of changes made to the OSM data, and this property indicates which changeset the entity belongs to.
    /// </summary>
    public long? ChangesetId { get; set; }

    /// <summary>
    /// Gets or sets the version of the OSM entity. Each time an entity is modified, its version number is incremented to reflect the changes made.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the user who last modified the OSM entity. This property indicates which user made the most recent changes to the entity.
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the OSM entity is visible.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last modification of the OSM entity. This property indicates when the entity was last updated in the OSM database.
    /// </summary>
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the collection of tags associated with the OSM entity. Tags provide additional information about the entity, such as its name, type, or other attributes.
    /// </summary>
    public ICollection<OsmTag> Tags { get; set; } = new List<OsmTag>();

    // OsmNode-specific properties

    /// <summary>
    /// Gets or sets the longitude coordinate of the OSM node. Longitude is east-west.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets the latitude coordinate of the OSM node. Latitude is north-south.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the collection of way nodes associated with the OSM node. A way node represents a node that is part of a way, which is a sequence of nodes that define a linear feature such as a road or path.
    /// </summary>
    public ICollection<WayNode> WayNodes { get; set; } = new List<WayNode>();

    /// <summary>
    /// Gets or sets the collection of relation members associated with the OSM node. A relation member represents a node that is part of a relation, which is a group of elements that define a complex feature or relationship.
    /// </summary>
    public ICollection<RelationMember> RelationMembers { get; set; } = new List<RelationMember>();
}
