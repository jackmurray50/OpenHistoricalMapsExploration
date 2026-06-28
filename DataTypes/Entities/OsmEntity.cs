namespace DataTypes.Entities;

/// <summary>
/// Represents a base class for OpenStreetMap (OSM) entities. This class contains common properties that are shared by all OSM entities, such as nodes, ways, and relations. It serves as a foundation for more specific OSM entity classes.
/// </summary>
public abstract class OsmEntity
{
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
}
