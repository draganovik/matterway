using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Matterway.ServiceDefaults.Bootstraps;

public static class BuilderBootstrap
{
    public static WebApplicationBuilder CreateBuilder(string[] args, bool? throwOnBadRequest = null)
    {
        var contentRootPath = Directory.GetCurrentDirectory();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = contentRootPath
        });

        builder.Configuration
            .SetBasePath(contentRootPath)
            .AddJsonFile("Properties/appsettings.json", true, true)
            .AddEnvironmentVariables();

        if (throwOnBadRequest is { } throwOnBadRequestValue)
            builder.Services.Configure<RouteHandlerOptions>(routeHandlerOptions =>
                routeHandlerOptions.ThrowOnBadRequest = throwOnBadRequestValue);

        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(jsonOptions =>
        {
            jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddValidation();
        builder.AddServiceDefaults();

        return builder;
    }
}