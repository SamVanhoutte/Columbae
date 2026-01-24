using System.Collections.Generic;

namespace Columbae;

public class MultiPolygon
{
    public MultiPolygon()
    {
    }

    public MultiPolygon(List<Polygon> areas)
    {
        Polygons = areas;
    }

    public List<Polygon> Polygons { get; set; }
    
}