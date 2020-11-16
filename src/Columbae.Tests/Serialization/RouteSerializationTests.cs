using System;
using System.IO;
using Columbae.World;
using Newtonsoft.Json;
using Xunit;

namespace Columbae.Tests.Serialization
{
    public class RouteSerializationTests
    {
        [Fact]
        public void Route_Serialization_ShouldWork()
        {
            // arrange
            var route = GetRoute("route01.json");

            // act
            var serializer = new JsonSerializer();
            var sWriter = new StringWriter();
            var writer = new JsonTextWriter(sWriter);
            serializer.Serialize(writer, route);
            
            Assert.NotNull(writer.ToString());
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