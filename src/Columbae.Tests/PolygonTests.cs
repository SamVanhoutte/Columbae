using System.Drawing;
using System.Linq;
using Xunit;

namespace Columbae.Tests
{
    public class PolygonTests
    {
        [Theory]
        [InlineData("0,0,0,2,2,2,2,0", "0.01,1.99")] // Close to the edge
        [InlineData("0,0,0,2,2,2,2,0", "2,2")] // On the vertex
        [InlineData("0,0,0,2,2,2,2,0", "0,0")] // On the vertex
        [InlineData("0,0,0,2,2,2,2,0", "0,2")] // On the vertex
        [InlineData("0,0,0,2,2,2,2,0", "2,0")] // On the vertex
        [InlineData("0,0,0,2,2,2,2,0", "1,0")] // On the edge
        [InlineData("0,0,0,2,2,0", "1,0")] // On triangle
        [InlineData("0,0,0,2,2,2,2,0", "1,1")] // Inside of square
        [InlineData("0,0,2,0,2,2,0,2", "3,2", false)] // Edge
        [InlineData("0,1,2,0,1,3,3,3,2,2", "1,1")] // Inside of polygon
        public void Polygon_IsInside_ShouldMatch(string polygonStr, string pointToMatch, bool shouldBeInside = true)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);
            var matchingPoint = Polypoint.Parse(pointToMatch);

            // act
            var isContained = polygon.IsInside(matchingPoint);

            // assert
            Assert.Equal(shouldBeInside, isContained);
        }

        [Theory]
        [InlineData("-1,-1,-4,-1,-4,-1,-4,-4", "-2,-2,-3,-3")] // Negative fully inside
        [InlineData("0,1,0,4,3,4,3,1", "0,2,4,3")] // Start & stop outside, but crossing
        [InlineData("0,1,0,4,3,4,3,1", "1,2,2,3")] // Fully inside
        [InlineData("0,1,0,4,3,4,3,1", "1,2,6,3")] // Start inside, end outside
        [InlineData("0,1,0,4,3,4,3,1", "0,0,1,3")] // Start outside, end inside
        [InlineData("0,1,0,4,3,4,3,1", "-1,-2,1,3")] // Start outside, end inside
        [InlineData("0,1,0,4,3,4,3,1", "1,3.5,3,10")] // Start & stop outside, but crossing
        public void Polygon_Intersects_ShouldMatch(string polygonStr, string segmentToMatch)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);
            var matchingSegment = Polysegment.Parse(segmentToMatch);

            // act
            var intersects = polygon.Intersects(matchingSegment);

            // assert
            Assert.True(intersects);
        }

        [Theory]
        [InlineData("-1,-1,-4,-1,-4,-1,-4,-4", "2,2,3,3")]
        public void Polygon_Intersects_ShouldNotMatch(string polygonStr, string segmentToMatch)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);
            var notMatchingSegment = Polysegment.Parse(segmentToMatch);

            // act
            var intersects = polygon.Intersects(notMatchingSegment);

            // assert
            Assert.False(intersects);
        }

        [Theory]
        [InlineData("0,1,0,4,3,4,3,1", "1,3.5,3,10", false)] // Start & stop outside, but crossing
        [InlineData("0,1,0,4,3,4,3,1", "1,3.5,2,1.5", true)] // Start & stop outside, but crossing
        public void Polygon_Covers_Segment_ShouldMatch(string polygonStr, string segmentToMatch, bool shouldCover)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);
            var matchingSegment = Polysegment.Parse(segmentToMatch);

            // act
            var intersects = polygon.Covers(matchingSegment);

            // assert
            Assert.Equal(shouldCover, intersects);
        }
        
        [Theory]
        [InlineData("0,0,2,0,2,2,0,2", "2,2,0,2", 4)]
        [InlineData("0,0,2,2,2,0,0,2", "2,2,2,0", 4)]
        [InlineData("0,0,2,2,2,0,0,2", "0,2,0,0", 4)]
        public void Polygon_Sections_Shouldmatch(string polygonStr, string expectedSection, int expectedSections)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);
            var expectedSegment = Polysegment.Parse(expectedSection);

            // act
            var sections = polygon.Sections;

            // assert
            Assert.Equal(expectedSections, sections.Count);
            Assert.Contains(expectedSegment, sections);
        }

        [Theory]
        [InlineData("0,1, -3,1, -1,-1.5, 3,2.5, -2,3, 2,-1, -1,2", "-3,3,3,3,3,-1.5,-3,-1.5")]
        public void Polyline_Boundingbox_Shouldmatch(string polygonStr, string expectedBox)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);
            var expectedMatch = Polygon.ParseCsv(expectedBox);

            // act
            var box = polygon.BoundingBox;

            // assert
            Assert.Equal(expectedMatch, box);
        }

        [Theory]
        [InlineData("mlwlH{dt\\co~BghjB", true)] // Outside to inside
        [InlineData("ealkHs{hYdlG{`sCmqn@pg{Aimy@|llB}mz@|`l@_tSgzcBo~Ti`]", true)] // Outside to inside
        [InlineData("klavHu}nWcwEquaBl{d@khCxnj@|p_Bqem@uqG", true)] // Inside
        [InlineData("cstlHyenS|pg@gl}Aq}h@smT", false)] // Outside
        [InlineData("}gstH_~bYpC{MyAwMePoY{@qGhHuYuB{EoUgAiT~KaTRy[lYoTdY{YoJwBzJiFhBjAdLi@~B}a@jc@cLzAuJzJjJlJm@jDgIrAqM~Lk\\|KcPzYcQpg@qDjOtPlKzPr`@pO~JnHlL|Nz~@xIhQpJra@rQnTrMvu@|@ff@zA~KcSpJmT~NcTzZqJ`VsQhOuKxV}HhUyNnm@uCtFnKbWkG`JiNxJcNrU}PF_MfYaMpP{Svy@nArFy@rJeHnPmEbVZ`H|GlXeC`Ieb@mMn@d|@gLa@oPdTaIs@wCxCeIyNuDpEu@xH`AjGeA`CiCDaFzFkFmN{JfM_OdBaErKkQzLqBbMgXxb@kHpXgSrZcQxOuMv^kVfVmThm@oNtt@oKfHuQ~XoJv@ma@oi@c\\zAgVs^gKfCsB}PuCuKoPcWeAg@cFvLgApHd@jSqIxQaCrNpJjo@~Cbj@_Eff@yGt_@r@lr@`It_AiA~`@|DdQGbLvD~XV~O_GxjAvAzJu@`M|@zHcCbMeA|XkUn]`_@xc@kEvUx@tVoQvWkZpQoLzPqJdCcIrSmK`M_ErSArLaFdE{CpMwJpNcCbQ{PvJyNvT{Q|P{WoBuFdCaC}B_NdPwHQ}G|Wn@nFuBdWr@dYaEf]_@pPvAd]tGni@~@r]yEtPaPzNqS``@vEjt@jBpIqE`TPnV{BtNqPvT~NnXmMh]{Ij]uKvHqHxK]tIjAtNoAb`@nB`VrCfLd@z_AzCzOzCrGtNdJdK|h@lGu@xDdIrGnCfF`MlMlH`QpXcR~RsBhFZ|BgDnDuFvO_D`@}FgHgF|NdC`NoKlO`IjWWfQgD`Jz\\bkCjH|YxErg@|Id_@vDfe@?lX|Fbd@bHjJfQxFvB~RrKvRz\\tE`RfKhBjFlCrb@fJzQpBkAag@flAsNzz@qFzr@_ChBsVwAsJvj@aFnBoCdHMlO`B`Qw@fUrAhsAoAt^wPrjC}Gd_@oDhk@cUb|@cCfSdY``CpIbaA|GzM|VjPnIvSvGdB{Gv]iEbd@~Qx`@vVfd@uBnS@hIxIda@{D|Ri@l|@|Av{@aCpUyN`WvKfVcCp_@pCj\\zDdMcBhImAxf@}Lxl@}F~Ky@xL|A|WwMrJeAhTmSgD|@hb@Lrg@vDbRbFjGmBtBzXpfAjLbo@|Etb@vRjpCvDvUvFvPdIvM`OpLbI|A_AfSsFnQ~D`Od^`g@`FsIdLnT~@dLxKpe@b@dg@uChKlInHfD`JaB~JxQzg@pFt@nTlo@v@~NmLvI}BxE", true)]
        public void Polygon_Intersects_ShouldBeInBelgium(string polylineStr, bool shouldMatch)
        {
            // arrange
            var polyline = Polyline.ParsePolyline(polylineStr);
            var belgium = GeoConstants.BelgiumRough;

            // act
            var intersects = belgium.Intersects(polyline);

            // assert
            Assert.Equal(shouldMatch, intersects);
        }
        
        


        [Theory]
        [InlineData("klavHu}nWcwEquaBl{d@khC", true)] // Inside
        [InlineData("klavHu}nWcwEquaBl{d@khCajUhyY", true)]
        [InlineData("klavHu}nWcwEquaBl{d@khCbkt@nwnC", true, Skip = "TODO: should be fixed in later release")]
        public void Polygon_Line_ShouldBeCoveredByBelgium(string polylineStr, bool shouldMatch)
        {
            // arrange
            var polyline = Polyline.ParsePolyline(polylineStr);
            var belgium = GeoConstants.BelgiumRough;

            // act
            var covers = belgium.Covers(polyline);

            // assert
            Assert.Equal(shouldMatch, covers);
        }
        
        [Theory]
        [InlineData("0,0,0,3,3,5,3,3,3,0", "2,2,1.5,2.5,1,1")] // Inside
        public void Polygon_Line_ShouldBeCovered(string polygonStr, string lineStr)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);
            var line = Polyline.ParseCsv(lineStr);

            // act
            var covers = polygon.Covers(line);

            // assert
            Assert.True(covers);
        }

        [Theory]
        [InlineData("0,1,0,4,3,4,3,1", "1,3.5,3,10,3,3", false)] // Start & stop outside, but crossing
        [InlineData("0,1,0,4,3,4,3,1", "1,3.5,2,1.5,1,1.2", true)] // Start & stop outside, but crossing
        public void Polygon_Covers_Line_ShouldMatch(string polygonStr, string segmentToMatch, bool shouldCover)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);
            var line = Polyline.ParseCsv(segmentToMatch);

            // act
            var intersects = polygon.Covers(line);

            // assert
            Assert.Equal(shouldCover, intersects);
        }
        
        [Theory]
        [InlineData("0,0,0,4,1,3,4,4", 4, 4, 0)]
        [InlineData("-1,-1,-4,-1,-4,-1,-4,-4", 4, -4, -1)]
        [InlineData("0,1, -3,1, -1,-1.5, 3,2.5, -2,3, 2,-1, -1,2", 7, -1, 1)]
        public void Polygon_Parse_ShouldMatch(string polygonStr, int expectedVertices, double lastLongitude,
            double firstLatitude)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);

            // assert
            Assert.NotNull(polygon);
            Assert.Equal(expectedVertices, polygon.Vertices.Count);
            Assert.Equal(lastLongitude, polygon.Vertices.Last().X);
            Assert.Equal(firstLatitude, polygon.Vertices.First().Y);
        }


        [Theory]
        [InlineData("0,0,0,4,1,3,4,6,7")]
        [InlineData("0,0,0,4")]
        [InlineData("0,0,0,4,1,3,d,6,7")]
        public void Polygon_Parse_IncorrectValues_ShouldBeNull(string polygonStr)
        {
            // arrange
            var polygon = Polygon.ParseCsv(polygonStr);

            // assert
            Assert.Null(polygon);
        }
    }
}