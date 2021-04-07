using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Columbae.World;
using Xunit;

namespace Columbae.Tests
{
    public class GpxRoutingTests
    {
        [Fact]
        public void Test_ImportGpx_SingleTrack_ShouldWork()
        {
            // arrange
            var routeStream = GetRoute("singletrack.gpx");

            // act
            var gpx = Route.ImportGpx(routeStream);

            // assert
            Assert.NotNull(gpx);
            Assert.Single(gpx);
            Assert.Equal(55, gpx.First().Vertices.Count);
            Assert.Equal("Alsemberg", gpx.First().Name);
        }

        [Fact]
        public void Test_ImportGpx_MultiTrack_ShouldWork()
        {
            // arrange
            var routeStream = GetRoute("multitrack.gpx");

            // act
            var gpx = Route.ImportGpx(routeStream);

            // assert
            Assert.NotNull(gpx);
            Assert.Equal(2, gpx.Count());
            Assert.Equal(65, gpx.Last().Vertices.Count);
            Assert.Equal("Holstraat", gpx.Last().Name);
        }

        private StreamReader GetRoute(string routeName)
        {
            var asm = GetType().Assembly;
            var resourceName = $"{asm.GetName().Name}.Resources.{routeName}";
            var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception($"Unable to get manifest resource stream!: {resourceName}");
            return new StreamReader(stream);
        }
    }
}