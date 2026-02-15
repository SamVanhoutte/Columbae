using System.Linq;
using Columbae.World;
using Xunit;

namespace Columbae.Tests
{
    public class MultiPolygonTests
    {
        [Fact]
        public void TestMultiPolygon()
        {
            var multiPolygon = new MultiPolygon(new System.Collections.Generic.List<Polygon>
            {
                new(new System.Collections.Generic.List<Polypoint>
                {
                    new(0D, 0D),
                    new(1D, 0D),
                    new(1D, 1D),
                    new(0D, 1D)
                }),
                new(new System.Collections.Generic.List<Polypoint>
                {
                    new(2D, 2D),
                    new(3D, 2D),
                    new(3D, 3D),
                    new(2D, 3D)
                })
            });

            var polylineString = multiPolygon.ToPolylineString();
            var parsedMultiPolygon = MultiPolygon.ParsePolyline(polylineString);

            Assert.Equal(multiPolygon.Polygons.Count, parsedMultiPolygon.Polygons.Count);
            Assert.Equal(2, parsedMultiPolygon.Polygons.Count);
            for (int i = 0; i < multiPolygon.Polygons.Count; i++)
            {
                Assert.Equal(4, parsedMultiPolygon.Polygons[i].Vertices.Count);
                Assert.Equal(multiPolygon.Polygons[i].Vertices.Count, parsedMultiPolygon.Polygons[i].Vertices.Count);
                for (int j = 0; j < multiPolygon.Polygons[i].Vertices.Count; j++)
                {
                    Assert.Equal(multiPolygon.Polygons[i].Vertices[j].Latitude,
                        parsedMultiPolygon.Polygons[i].Vertices[j].Latitude, precision: 5);
                    Assert.Equal(multiPolygon.Polygons[i].Vertices[j].Longitude,
                        parsedMultiPolygon.Polygons[i].Vertices[j].Longitude, precision: 5);
                }
            }
        }

        [Fact]
        public void TestArray()
        {
            var multiPolygon = new MultiPolygon(new System.Collections.Generic.List<Polygon>
            {
                new(new System.Collections.Generic.List<Polypoint>
                {
                    new(0D, 0D),
                    new(1D, 0D),
                    new(1D, 1D),
                    new(0D, 1D)
                }),
                new(new System.Collections.Generic.List<Polypoint>
                {
                    new(2D, 2D),
                    new(3D, 2D),
                    new(3D, 3D),
                    new(2D, 3D)
                })
            });
            var array = multiPolygon.ToArray();
            Assert.Equal(2, array.Length);
            var polylineString = multiPolygon.ToPolylineString();
            var parsedMultiPolygon = MultiPolygon.ParsePolyline(polylineString);
            var parsedArray = parsedMultiPolygon.ToArray();
            Assert.Equal(2, parsedArray.Length);
        }
    }
}