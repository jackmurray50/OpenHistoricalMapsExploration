namespace DataTypes.Entities;

/// <summary>
/// Represents a node that is part of a way in OpenStreetMap (OSM) data. A way node defines the relationship between a way and its constituent nodes, including the order of the nodes within the way. Each way node has an associated sequence number that defines its position within the way.
/// </summary>
public class WayNode
{
    /// <summary>
    /// Gets or sets the unique identifier for the way node.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated OSM way. This property indicates which way the node belongs to.
    /// </summary>
    public long WayId { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the associated OSM node. This property indicates which node is part of the way.
    /// </summary>
    public long NodeId { get; set; }

    /// <summary>
    /// Gets or sets the sequence number of the node within the way. The sequence number defines the order of the node in the way.
    /// </summary>
    public int SequenceNumber { get; set; }

    // Navigation properties

    /// <summary>
    /// Gets or sets the navigation property for the associated OSM way. This property allows access to the way that the node belongs to.
    /// </summary>
    public OsmWay Way { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property for the associated OSM node. This property allows access to the node that is part of the way.
    /// </summary>
    public OsmNode Node { get; set; } = null!;
}
