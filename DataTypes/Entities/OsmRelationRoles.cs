namespace DataTypes.Entities;

/// <summary>
/// Common OpenStreetMap relation member roles.
/// See: https://wiki.openstreetmap.org/wiki/Relation.
/// </summary>
public static class OsmRelationRoles
{
    // Warnings are disabled for this file because it contains only constant string definitions, which are self-explanatory and do not require XML documentation comments.
#pragma warning disable CS1591 // Disable missing XML comment warnings
#pragma warning disable SA1600
    // Multipolygon roles
    public const string Inner = "inner";
    public const string Outer = "outer";

    // Route roles
    public const string Forward = "forward";
    public const string Backward = "backward";
    public const string From = "from";
    public const string To = "to";
    public const string Via = "via";
    public const string Start = "start";
    public const string End = "end";
    public const string Stop = "stop";
    public const string Platform = "platform";

    // Directional roles
    public const string North = "north";
    public const string South = "south";
    public const string East = "east";
    public const string West = "west";

    // Hierarchical roles
    public const string Subarea = "subarea";
    public const string Subrelation = "subrelation";
    public const string Link = "link";

    // Administrative/organizational roles
    public const string AdminCentre = "admin_centre";
    public const string Label = "label";
    public const string Archive = "archive";
    public const string Operator = "operator";
    public const string Network = "network";

    // Restriction roles
    public const string Restriction = "restriction";

    // Other common roles
    public const string Member = "member";
    public const string Location = "location";
    public const string Branch = "branch";
}
#pragma warning restore CS1591 // Restore missing XML comment warnings
#pragma warning restore SA1600
