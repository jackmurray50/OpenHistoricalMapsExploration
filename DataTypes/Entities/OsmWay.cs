namespace DataTypes.Entities;

/// <summary>
/// Represents a way in OpenStreetMap (OSM) data. A way is a sequence of nodes that define a linear feature, such as a road, path, or boundary. Ways can have associated tags that provide additional information about the way, and they can be part of relations.
/// </summary>
public class OsmWay : OsmEntity
{
    // Navigation properties

    /// <summary>
    /// Gets or sets the collection of way nodes associated with the OSM way. A way node represents a node that is part of a way, which is a sequence of nodes that define a linear feature such as a road or path.
    /// </summary>
    public ICollection<WayNode> WayNodes { get; set; } = new List<WayNode>();

    /// <summary>
    /// Gets or sets the collection of relation members associated with the OSM way. A relation member represents a way that is part of a relation, which is a group of elements that define a complex feature or relationship.
    /// </summary>
    public ICollection<RelationMember> RelationMembers { get; set; } = new List<RelationMember>();
}
