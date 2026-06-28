namespace DataTypes.Entities;

/// <summary>
/// Represents a relation in OpenStreetMap (OSM) data. A relation is a group of elements (nodes, ways, or other relations) that define a complex feature or relationship. Relations can have associated tags that provide additional information about the relation.
/// </summary>
public class OsmRelation : OsmEntity
{
    /// <summary>
    /// Gets or sets the collection of members associated with the OSM relation. A member represents an element (node, way, or relation) that is part of the relation.
    /// </summary>
    public ICollection<RelationMember> Members { get; set; } = new List<RelationMember>();
}
