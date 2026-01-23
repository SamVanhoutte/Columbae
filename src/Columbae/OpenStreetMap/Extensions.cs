using System;
using System.Collections.Generic;
using System.Linq;
using Columbae.OpenStreetMap.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Columbae.OpenStreetMap;

public static class Extensions
{
    public static IServiceCollection AddOpenStreetMapIntegration(this IServiceCollection services)
    {
        services.AddScoped<IOverpassApiClient, OverpassApiClient>();
        services.AddHttpClient<IOverpassApiClient, OverpassApiClient>(client =>
        {
            client.BaseAddress = OverpassApiClient.BaseUrl;
        });
        return services;
    }

    public static List<Polygon> ToPolygons(this OsmMember[] osmGeometries)
    {
        var polygons = osmGeometries.Select(g =>
            new Polygon(g.Geometries.Select(gm => new Polypoint(gm.Lon, gm.Lat)).ToList()));
        return polygons.ToList();
    }
}