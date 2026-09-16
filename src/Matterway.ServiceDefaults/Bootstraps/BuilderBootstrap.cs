using System.Text.Json.Serialization;
using Matterway.ServiceDefaults.Identifiers;
using Matterway.ServiceDefaults.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Matterway.ServiceDefaults.Bootstraps;

public static class BuilderBootstrap
{
    public static WebApplicationBuilder CreateBuilder(string[] args, bool? throwOnBadRequest = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (throwOnBadRequest is { } throwOnBadRequestValue)
            builder.Services.Configure<RouteHandlerOptions>(routeHandlerOptions =>
                routeHandlerOptions.ThrowOnBadRequest = throwOnBadRequestValue);

        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(jsonOptions =>
        {
            jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.Configure<RouteOptions>(routeOptions =>
        {
            routeOptions.ConstraintMap[nameof(ArticleCode)] = typeof(ParsableRouteConstraint<ArticleCode>);
            routeOptions.ConstraintMap[nameof(OrderId)] = typeof(ParsableRouteConstraint<OrderId>);
        });

        builder.AddServiceDefaults();

        return builder;
    }
}