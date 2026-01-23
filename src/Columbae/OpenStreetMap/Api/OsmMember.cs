using System.Text.Json.Serialization;

namespace Columbae.OpenStreetMap.Api;

public class OsmMember
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("ref")]
    public int Ref { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("geometry")]
    public Geometry[] Geometries { get; set; }
}