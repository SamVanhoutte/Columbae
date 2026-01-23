using System.Collections.Generic;
using System.Text;
using Columbae.OpenStreetMap.Api;

namespace Columbae.OpenStreetMap;

public static class OverpassQueryBuilder
{
    /// <summary>
    /// Build Overpass QL query for nodes with specific tags in a bounding box
    /// </summary>
    public static string BuildSearchTagsInBoxQuery(BoundingBox boundingBox, List<OsmTag> tags, int timeout = 180)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[out:json][timeout:{timeout}];");
        sb.Append("(");

        foreach (var tag in tags)
        {
            // Query for nodes with this tag in the bounding box
            sb.AppendLine();
            sb.Append($"  node[\"{tag.Key}\"=\"{tag.Value}\"]({boundingBox.ToOverpassBbox()});");
        }

        sb.AppendLine();
        sb.AppendLine(");");
        sb.AppendLine("out body;");

        return sb.ToString();
    }

    /// <summary>
    /// Build Overpass QL query for specific tag by name
    /// </summary>
    public static string BuildSearchTagByName(string name, List<OsmTag> tags, bool caseSensitive = true, int timeout = 180)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[out:json][timeout:{timeout}];");
        sb.Append("(");
        foreach (var tag in tags)
        {
            // Query for nodes with this tag in the bounding box
            sb.AppendLine();
            sb.Append(caseSensitive ? 
                $"  nwr[\"name\"=\"{name}\"][\"{tag.Key}\"=\"{tag.Value}\"];":
                $"  nwr[\"name\"~\"{name}\",i][\"{tag.Key}\"=\"{tag.Value}\"];");
        }
        sb.AppendLine();
        sb.AppendLine(");");
        sb.AppendLine("out geom;");

        return sb.ToString();
    }
}