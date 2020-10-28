using System;
using System.IO;
using System.Threading.Tasks;
using Columbae.GeoJson;
using Columbae.Routing;
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
            var box = polyline.BoundingBox;
            var outputString = await route.ExportGpx();
            Assert.NotNull(outputString);
        }

        [Theory]
        [InlineData("route01.json", "}cwrHkzyc@VFj@f@^?LGZe@b@a@nAaAd@o@`@y@n@uCL{@BqCJ_BHu@Nw@LKLRPb@FHFBV?|Bs@HB~@jCl@|GLz@Pl@lBdFnExIp@|Ah@`BxAbDTZ`@RDb@VtAHv@`DhJ", false)] // Cote Floriheid, outside of route
        [InlineData("route01.json", "moduHg}iU{@rESbAM\\\\QVSRgBpAOPMRGTKr@]hEMl@]~@W^mBjBaA|A]\\\\sCxBi@XQLWl@WlAYjA[|AWh@YPO@i@Ky@Ee@Se@EWBs@RuBLm@Gg@Mw@M", true)] // Kapelleberg, ridden
        [InlineData("route01.json", "{lfuHgvjU[j@GR[f@c@`A_AbCwBrEOr@a@jDq@|DGj@", false)] // Hauwaert, inside route, not ridden
        public async Task Route_Contains_Section_ShouldWork(string routeName, string segmentPolystring, bool shouldContain)
        {
            var route = GetRoute(routeName);
            var segment = Polygon.ParsePolyline(segmentPolystring);
            Assert.NotNull(route);
            Assert.NotNull(segment);

            Assert.Equal(shouldContain, route.Contains(segment, 0.002));
        }

        private Route GetRoute(string routeName)
        {
            var asm = GetType().Assembly;
            var resourceName = $"{asm.GetName().Name}.Resources.{routeName}";
            using var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception($"Unable to get manifest resource stream!: {resourceName}");
            var geoLine = Geoline.Parse(stream);
            return new Route(geoLine.Vertices);
        }
    }
}