namespace Columbae.OpenStreetMap.Api;

/// <summary>
/// Geographic bounding box for OSM import region
/// </summary>
public class BoundingBox
{
    /// <summary>
    /// Minimum latitude (south boundary)
    /// </summary>
    public double MinLatitude { get; set; }

    /// <summary>
    /// Minimum longitude (west boundary)
    /// </summary>
    public double MinLongitude { get; set; }

    /// <summary>
    /// Maximum latitude (north boundary)
    /// </summary>
    public double MaxLatitude { get; set; }

    /// <summary>
    /// Maximum longitude (east boundary)
    /// </summary>
    public double MaxLongitude { get; set; }

    /// <summary>
    /// Convert to Overpass API bbox format: (south, west, north, east)
    /// Uses InvariantCulture to ensure decimal points regardless of regional settings
    /// </summary>
    public string ToOverpassBbox()
    {
        return $"{MinLatitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{MinLongitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{MaxLatitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{MaxLongitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
    }

}
