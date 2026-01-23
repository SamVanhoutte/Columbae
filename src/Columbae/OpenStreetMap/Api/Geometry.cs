using System.Text.Json.Serialization;

namespace Columbae.OpenStreetMap.Api;

public class Geometry
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }
    [JsonPropertyName("lon")]
    public double Lon { get; set; }
}