using System;
using System.Collections.Generic;
using System.Linq;

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

    public double[][][][] ToArray()
    {
        return [Polygons.Select(p=>p.ToArray()).ToArray()];
    }
    public string ToPolylineString()
    {
        return string.Join(Environment.NewLine, Polygons.Select(p => p.ToPolylineString()));
    }

    public static MultiPolygon ParsePolyline(string polyline)
    {
        return new MultiPolygon( polyline.Split(Environment.NewLine).Select(Polygon.ParsePolyline).ToList());
    }
    
}