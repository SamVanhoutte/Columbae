using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Columbae.OpenStreetMap.Api;

namespace Columbae.OpenStreetMap;

public class RegionQueryResults
{
    [JsonPropertyName("tags")]
    public Dictionary<string, string>? Tags { get; set; }

    [JsonPropertyName("region")]
    public Polygon[] Region { get; set; }

    internal static RegionQueryResults Empty => new RegionQueryResults();
    
    internal static RegionQueryResults FromOverpass(OverpassResponse response)
    {
        var regionElement = response.Elements.FirstOrDefault();
        if (regionElement == null) return RegionQueryResults.Empty;
        return new RegionQueryResults
        {
            Tags = regionElement.Tags,
            Region = regionElement.Members.ToPolygons().ToArray()
        };
    }
}