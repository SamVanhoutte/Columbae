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
            var polyline = new Polyline(polylineString);
            var route = new Route(polyline.Points);
            var outputString = await route.ExportGpx();
            Assert.NotNull(outputString);
        }
    }
}