using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Columbae.OpenStreetMap.Api;

/// <summary>
/// OSM element (node, way, or relation)
/// </summary>
public class OsmElement
{
    /// <summary>
    /// Element type (node, way, relation)
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// OSM element ID
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Bounds of the response
    /// </summary>
    [JsonPropertyName("bounds")]
    public OsmBounds? Bounds { get; set; }
    
    /// <summary>
    /// Bounds of the response
    /// </summary>
    [JsonPropertyName("members")]
    public OsmMember[]? Members { get; set; }
    
    /// <summary>
    /// Latitude (for nodes with coordinates)
    /// </summary>
    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    /// <summary>
    /// Longitude (for nodes with coordinates)
    /// </summary>
    [JsonPropertyName("lon")]
    public double? Lon { get; set; }

    /// <summary>
    /// OSM tags (key-value pairs)
    /// </summary>
    [JsonPropertyName("tags")]
    public Dictionary<string, string>? Tags { get; set; }
}
