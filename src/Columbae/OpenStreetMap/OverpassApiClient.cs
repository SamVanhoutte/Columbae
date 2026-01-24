using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Columbae.OpenStreetMap.Api;
using Microsoft.Extensions.Logging;

namespace Columbae.OpenStreetMap;

/// <summary>
/// HTTP client for querying OpenStreetMap Overpass API
/// </summary>
public class OverpassApiClient(
    HttpClient httpClient,
    ILogger<OverpassApiClient> logger)
    : IOverpassApiClient
{
    private static readonly ActivitySource activitySource = new("Columbae.OverpassApiClient");
    public static Uri BaseUrl => new Uri("https://overpass-api.de/api/interpreter/");

    // public OverpassApiClient()
    // {
    //     httpClient = new HttpClient();
    //     httpClient.Timeout = TimeSpan.FromSeconds(300);
    //     httpClient.BaseAddress = new Uri("https://overpass-api.de/api/interpreter/");
    //     //logger = LoggerFactory.Create(builder => {}).CreateLogger(typeof(OverpassApiClient));
    //     logger = LoggerFactory.Create(builder => {})
    //         .CreateLogger<OverpassApiClient>();
    // }

    /// <summary>
    /// Execute an Overpass QL query and return OSM elements
    /// </summary>
    public async Task<OverpassResponse> QueryAsync(string query, CancellationToken cancellationToken = default)
    {
        using var activity = activitySource.StartActivity("OverpassApiQuery");
        activity?.SetTag("query.length", query.Length);

        var startTime = DateTime.UtcNow;

        try
        {
            logger.LogInformation("Executing Overpass API query: {QueryPreview}...",
                query);

            var content = new StringContent(query, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync("", content, cancellationToken);

            var duration = DateTime.UtcNow - startTime;
            activity?.SetTag("http.status_code", (int)response.StatusCode);
            activity?.SetTag("duration_ms", duration.TotalMilliseconds);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Overpass API request failed with status {StatusCode}. Duration: {Duration}ms. Error: {Error}",
                    response.StatusCode,
                    duration.TotalMilliseconds,
                    errorBody.Length > 500 ? errorBody[..500] : errorBody);

                activity?.SetStatus(ActivityStatusCode.Error, $"HTTP {response.StatusCode}");
                throw new HttpRequestException(
                    $"Overpass API returned {response.StatusCode}: {errorBody}",
                    null,
                    response.StatusCode);
            }

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var overpassResponse = JsonSerializer.Deserialize<OverpassResponse>(responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (overpassResponse == null)
            {
                logger.LogError("Failed to deserialize Overpass API response");
                activity?.SetStatus(ActivityStatusCode.Error, "Deserialization failed");
                throw new InvalidOperationException("Failed to parse Overpass API response");
            }

            logger.LogInformation(
                "Overpass API query completed successfully. Elements: {ElementCount}, Duration: {Duration}ms",
                overpassResponse.Elements.Count,
                duration.TotalMilliseconds);

            activity?.SetTag("elements.count", overpassResponse.Elements.Count);

            return overpassResponse;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Overpass API query cancelled after {Duration}ms",
                (DateTime.UtcNow - startTime).TotalMilliseconds);
            activity?.SetStatus(ActivityStatusCode.Error, "Cancelled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Overpass API query failed after {Duration}ms",
                (DateTime.UtcNow - startTime).TotalMilliseconds);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Execute an Overpass QL query and return OSM elements
    /// </summary>
    public async Task<RegionQueryResults> QueryRegionAsync(string query, CancellationToken cancellationToken = default)
    {
        var overpassResponse = await QueryAsync(query, cancellationToken);
        return RegionQueryResults.FromOverpass(overpassResponse);
    }

    public static HttpClient CreateHttpClient()
    {
        return new HttpClient() { BaseAddress = BaseUrl };
    }
}