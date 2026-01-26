using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Columbae.OpenStreetMap.Api;

namespace Columbae.OpenStreetMap;

public class QueryResult
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("type")] public string ElementType { get; set; }
    [JsonPropertyName("tags")] public Dictionary<string, string>? Tags { get; set; }

}

public class RegionQueryResult: QueryResult
{
    
    [JsonPropertyName("region")] public MultiPolygon Region { get; set; }

    internal static RegionQueryResult Empty => new RegionQueryResult();

    internal static RegionQueryResult FromOverpass(OverpassResponse response)
    {
        var regionElement = response.Elements.FirstOrDefault(e => e.Members?.Any() ?? false);
        if (regionElement == null) return RegionQueryResult.Empty;
        return new RegionQueryResult
        {
            Id = regionElement.Id.ToString(), 
            ElementType = regionElement.Type,
            Tags = regionElement.Tags,
            Region = new MultiPolygon(regionElement.Members.ToPolygons().ToList())
        };
    }
}