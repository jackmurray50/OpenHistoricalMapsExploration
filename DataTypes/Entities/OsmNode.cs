namespace DataTypes.Entities;

/// <summary>
/// Represents a node in OpenStreetMap (OSM) data. A node is a single point defined by its latitude and longitude coordinates. Nodes can be part of ways and relations, and they can have associated tags that provide additional information about the node.
/// </summary>
public class OsmNode : OsmEntity
{
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
