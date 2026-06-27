using System.Text.Json.Serialization;
using Matterway.ServiceDefaults.Identifiers;
using Matterway.ServiceDefaults.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Matterway.ServiceDefaults.Bootstraps;

public static class BuilderBootstrap
{
    public static WebApplicationBuilder CreateBuilder(string[] args, bool? throwOnBadRequest = null)
    {
        var builder = WebApplication.CreateBuilder(args);
        var environmentName = builder.Environment.EnvironmentName;

        builder.Configuration.AddJsonFile("Properties/appsettings.json", true, true);
        builder.Configuration.AddJsonFile($"Properties/appsettings.{environmentName}.json", true, true);
        builder.Configuration.AddEnvironmentVariables();

        if (args.Length > 0)
            builder.Configuration.AddCommandLine(args);

        if (throwOnBadRequest is { } throwOnBadRequestValue)
            builder.Services.Configure<RouteHandlerOptions>(routeHandlerOptions =>
                routeHandlerOptions.ThrowOnBadRequest = throwOnBadRequestValue);

        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(jsonOptions =>
        {
            jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.Configure<RouteOptions>(routeOptions =>
        {
            routeOptions.ConstraintMap[nameof(ArticleCode)] = typeof(ArticleCodeRouteConstraint);
            routeOptions.ConstraintMap[nameof(OrderId)] = typeof(OrderIdRouteConstraint);
        });

        builder.AddServiceDefaults();

        return builder;
    }
}