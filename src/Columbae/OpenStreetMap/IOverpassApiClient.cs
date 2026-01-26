using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Columbae.OpenStreetMap.Api;

namespace Columbae.OpenStreetMap;

/// <summary>
/// HTTP client for querying OpenStreetMap Overpass API
/// Executes Overpass QL queries with retry logic and error handling
/// </summary>
public interface IOverpassApiClient
{
    /// <summary>
    /// Execute an Overpass QL query and parse the JSON response
    /// </summary>
    /// <param name="query">Overpass QL query string (e.g., node["tourism"="viewpoint"](bbox))</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parsed Overpass API response with OSM elements</returns>
    /// <exception cref="HttpRequestException">Thrown when API returns non-success status code</exception>
    /// <exception cref="InvalidOperationException">Thrown when response JSON cannot be parsed</exception>
    Task<OverpassResponse> QueryAsync(string query, CancellationToken cancellationToken = default);

    Task<RegionQueryResult> QueryRegionAsync(string query, CancellationToken cancellationToken = default);
}