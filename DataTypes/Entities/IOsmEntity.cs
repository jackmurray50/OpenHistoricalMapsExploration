namespace DataTypes.Entities;

/// <summary>
/// Represents a contract for OpenStreetMap (OSM) entities. This interface defines common properties that are shared by all OSM entities, such as nodes, ways, and relations.
/// </summary>
public interface IOsmEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the OSM entity.
    /// </summary>
    long Id { get; set; }

    /// <summary>
    /// Gets or sets the Changeset ID of the OSM entity. A changeset represents a group of changes made to the OSM data, and this property indicates which changeset the entity belongs to.
    /// </summary>
    long? ChangesetId { get; set; }

    /// <summary>
    /// Gets or sets the version of the OSM entity. Each time an entity is modified, its version number is incremented to reflect the changes made.
    /// </summary>
    int Version { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the user who last modified the OSM entity. This property indicates which user made the most recent changes to the entity.
    /// </summary>
    long? UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the OSM entity is visible.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last modification of the OSM entity. This property indicates when the entity was last updated in the OSM database.
    /// </summary>
    DateTime? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the collection of tags associated with the OSM entity. Tags provide additional information about the entity, such as its name, type, or other attributes.
    /// </summary>
    ICollection<OsmTag> Tags { get; set; }
}
