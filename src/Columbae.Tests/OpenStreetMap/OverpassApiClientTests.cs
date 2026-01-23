using System.Net.Http;
using System.Threading.Tasks;
using Columbae.OpenStreetMap;
using Columbae.OpenStreetMap.Api;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Columbae.Tests.OpenStreetMap;

/// <summary>
/// Unit tests for ImportJobProcessor transformation and validation logic
/// Tests coordinate validation, description building, and geometry creation
/// </summary>
public class OverpassApiClientTests
{
    [Fact]
    public async Task QueryNationalParkShouldReturnValidData()
    {
        IOverpassApiClient apiClient = new OverpassApiClient(new HttpClient(), new NullLogger<OverpassApiClient>());
        var parkName1 = "Yellowstone National Park";
        var query1 = OverpassQueryBuilder.BuildSearchTagByName(parkName1,
            [OsmTag.Parse("boundary=national_park"), OsmTag.Parse("leisure=nature_reserve"), 
                OsmTag.Parse("boundary=protected_area")], timeout: 180);
        var result1 = await apiClient.QueryAsync(query1);
        Assert.NotNull(result1);
        var parkName2 = "yellowstone national park";
        var query2 = OverpassQueryBuilder.BuildSearchTagByName(parkName2,
        [OsmTag.Parse("boundary=national_park"), OsmTag.Parse("leisure=nature_reserve"), 
            OsmTag.Parse("boundary=protected_area")], caseSensitive:false, timeout: 180);
        var result2 = await apiClient.QueryAsync(query2);
        Assert.NotNull(result2);
        Assert.Equal(result2.Elements.Count, result1.Elements.Count);
    }
    
    [Fact]
    public async Task QueryNationalParkShouldReturnValidRegion()
    {
        IOverpassApiClient apiClient = new OverpassApiClient(OverpassApiClient.CreateHttpClient(), new NullLogger<OverpassApiClient>());
        var parkName1 = "Yellowstone National Park";
        var query1 = OverpassQueryBuilder.BuildSearchTagByName(parkName1,
        [OsmTag.Parse("boundary=national_park"), OsmTag.Parse("leisure=nature_reserve"), 
            OsmTag.Parse("boundary=protected_area")], timeout: 180);
        var result1 = await apiClient.QueryRegionAsync(query1);
        Assert.NotNull(result1);
        Assert.NotEmpty(result1.Region);
    }
}
