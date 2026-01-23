
namespace Columbae.OpenStreetMap.Api;

/// <summary>
/// OSM tag filter (key=value)
/// </summary>
public class OsmTag
{
    /// <summary>
    /// OSM tag key (e.g., "tourism", "amenity")
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// OSM tag value (e.g., "viewpoint", "restaurant")
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Format as "key=value" string
    /// </summary>
    public override string ToString() => $"{Key}={Value}";

    public static OsmTag Parse(string line) => new OsmTag { Key = line.Split('=')[0], Value = line.Split('=')[1] };
}