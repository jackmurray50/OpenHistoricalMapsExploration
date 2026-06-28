namespace DataTypes.Entities;

/// <summary>
/// Represents a tag in OpenStreetMap (OSM) data. A tag is a key-value pair that provides additional information about an OSM element, such as a node, way, or relation. Tags are used to describe the characteristics and attributes of OSM elements.
/// </summary>
public class OsmTag
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
    /// Gets or sets the foreign key for the associated OSM entity. This property is nullable, as a tag may not be associated with an entity.
    /// </summary>
    public long? EntityId { get; set; }

    // Navigation properties

    /// <summary>
    /// Gets or sets the navigation property for the associated OSM Entity. This property allows navigation from the tag to the corresponding OSM entity (node, way, or relation) that the tag is associated with.
    /// </summary>
    public OsmEntity Entity { get; set; } = null!;
}
