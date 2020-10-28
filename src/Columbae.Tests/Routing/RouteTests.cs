using System;
using System.IO;
using System.Threading.Tasks;
using Columbae.World;
using Xunit;

namespace Columbae.Tests.Routing
{
    public class RouteTests
    {
        [Fact]
        public async Task Route_ExportGpx_ShouldWork()
        {
            var polylineString = "adeuHqjlUo@i@u@e@uAu@u@]sB]g@EoCe@MA_@BsA^_@Pe@DgBw@aAYkAk@}A[QBaAn@m@R";
            var polyline = Polygon.ParsePolyline(polylineString);
            var route = new Route(polyline.Vertices);
            var outputString = await route.ExportGpx();
            Assert.NotNull(outputString);
        }

        [Theory]
        [InlineData("route01.json", "}cwrHkzyc@VFj@f@^?LGZe@b@a@nAaAd@o@`@y@n@uCL{@BqCJ_BHu@Nw@LKLRPb@FHFBV?|Bs@HB~@jCl@|GLz@Pl@lBdFnExIp@|Ah@`BxAbDTZ`@RDb@VtAHv@`DhJ", false)] // Cote Floriheid, outside of route
        [InlineData("route01.json", @"moduHg}iU{@rESbAM\QVSRgBpAOPMRGTKr@]hEMl@]~@W^mBjBaA|A]\sCxBi@XQLWl@WlAYjA[|AWh@YPO@i@Ky@Ee@Se@EWBs@RuBLm@Gg@Mw@M", true)] // Kapelleberg, ridden
        [InlineData("route01.json", "{lfuHgvjU[j@GR[f@c@`A_AbCwBrEOr@a@jDq@|DGj@", false)] // Hauwaert, inside route, not ridden
        public void Route_Contains_Section_ShouldWork(string routeName, string segmentPolystring, bool shouldContain)
        {
            var route = GetRoute(routeName);
            var segment = Polygon.ParsePolyline(segmentPolystring);
            Assert.NotNull(route);
            Assert.NotNull(segment);
            var result = route.Contains(segment, 0.002);
            
            Assert.Equal(shouldContain, result);
        }
        
        [Theory]
        [InlineData("route01.json", @"moduHg}iU{@rESbAM\QVSRgBpAOPMRGTKr@]hEMl@]~@W^mBjBaA|A]\sCxBi@XQLWl@WlAYjA[|AWh@YPO@i@Ky@Ee@Se@EWBs@RuBLm@Gg@Mw@M", true)] // Kapelleberg, ridden
        [InlineData("route01.json", @"cteuH_ljUmDhC{A~B{@dE_@lEe@zDsDrD", false)] // Boigne, wrong direction
        public void Route_Contains_SectionWithDirection_ShouldWork(string routeName, string segmentPolystring, bool sameDirection)
        {
            var route = GetRoute(routeName);
            var segment = Polygon.ParsePolyline(segmentPolystring);
            Assert.NotNull(route);
            Assert.NotNull(segment);

            var resultWithoutDirection = route.Contains(segment, 0.002);
            Assert.True(resultWithoutDirection);
            var resultWithDirection = route.Contains(segment, 0.002, true);
            Assert.Equal(sameDirection, resultWithDirection);
        }

        [Theory]
        [InlineData("0,0,10,0,10,10,5,10,10,0,0,4", "2,0,6,0,10,1,10,3", true)] 
        [InlineData("0,0,10,0,10,10,5,10,10,0,0,4", "2,0,10,1", true)] 
        [InlineData("0,0,10,0,10,10,5,10,10,0,0,4", "10,1,2,0", false)] 
        public void Route_Contains_Points_SectionWithDirection_ShouldWork(string routeStr, string segmentStr, bool sameDirection)
        {
            var geoLine = Polyline.ParseCsv(routeStr);
            var route = new Route(geoLine.Vertices);
            var segment = Polyline.ParseCsv(segmentStr);
            Assert.NotNull(route);
            Assert.NotNull(segment);

            var resultWithoutDirection = route.Contains(segment);
            Assert.True(resultWithoutDirection);
            var resultWithDirection = route.Contains(segment, 0, true);
            Assert.Equal(sameDirection, resultWithDirection);
        }
        
        private Route GetRoute(string routeName)
        {
            var asm = GetType().Assembly;
            var resourceName = $"{asm.GetName().Name}.Resources.{routeName}";
            using var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception($"Unable to get manifest resource stream!: {resourceName}");
            using var reader = new StreamReader(stream);
            var geoLine = Polyline.ParseJson(reader.ReadToEnd());
            return new Route(geoLine.Vertices);
        }
    }
}