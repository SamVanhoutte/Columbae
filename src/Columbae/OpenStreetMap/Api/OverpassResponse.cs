using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Columbae.OpenStreetMap.Api;

/// <summary>
/// Overpass API response wrapper
/// </summary>
public class OverpassResponse
{
    /// <summary>
    /// API version
    /// </summary>
    [JsonPropertyName("version")]
    public double Version { get; set; }

    /// <summary>
    /// Generator identifier
    /// </summary>
    [JsonPropertyName("generator")]
    public string Generator { get; set; } = string.Empty;

    /// <summary>
    /// OSM3S metadata (timestamps, etc.)
    /// </summary>
    [JsonPropertyName("osm3s")]
    public Dictionary<string, object>? ServerStats { get; set; }

    /// <summary>
    /// Array of OSM elements (nodes, ways, relations)
    /// </summary>
    [JsonPropertyName("elements")]
    public List<OsmElement> Elements { get; set; } = [];
}
