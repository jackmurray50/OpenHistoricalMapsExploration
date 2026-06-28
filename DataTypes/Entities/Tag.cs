namespace DataTypes.Entities;

/// <summary>
/// Represents a tag in OpenStreetMap (OSM) data. A tag is a key-value pair that provides additional information about an OSM element, such as a node, way, or relation. Tags are used to describe the characteristics and attributes of OSM elements.
/// </summary>
public class Tag
{
    /// <summary>
    /// Gets or sets the unique identifier for the tag.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the key of the tag, which represents the attribute or property being described.
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Gets or sets the value of the tag, which provides the specific information or value associated with the key.
    /// </summary>
    public string Value { get; set; } = null!;

    // Foreign keys

    /// <summary>
    /// Gets or sets the foreign key for the associated OSM node. This property is nullable, as a tag may not be associated with a node.
    /// </summary>
    public long? NodeId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated OSM way. This property is nullable, as a tag may not be associated with a way.
    /// </summary>
    public long? WayId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated OSM relation. This property is nullable, as a tag may not be associated with a relation.
    /// </summary>
    public long? RelationId { get; set; }

    // Navigation properties

    /// <summary>
    /// Gets or sets the navigation property for the associated OSM node.
    /// </summary>
    public OsmNode? Node { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the associated OSM way.
    /// </summary>
    public OsmWay? Way { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the associated OSM relation.
    /// </summary>
    public OsmRelation? Relation { get; set; }
}
