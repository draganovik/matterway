using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Matterway.ServiceDefaults.Api;

public static partial class ApiTemplate
{
    public static WebApplicationBuilder CreateApiBuilder(
        string[] args,
        ApiBootstrapFeatureOptions? options = null)
    {
        options ??= new ApiBootstrapFeatureOptions();
        var contentRootPath = options.ContentRootPath ?? Directory.GetCurrentDirectory();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = contentRootPath
        });

        builder.Configuration
            .SetBasePath(contentRootPath)
            .AddJsonFile(
                string.Format(options.EnvironmentSettingsFilePattern, builder.Environment.EnvironmentName),
                options.EnvironmentSettingsOptional,
                options.EnvironmentSettingsReloadOnChange);

        if (options.AddEnvironmentVariables)
            builder.Configuration.AddEnvironmentVariables();

        if (options.ThrowOnBadRequest is { } throwOnBadRequest)
            builder.Services.Configure<RouteHandlerOptions>(routeHandlerOptions =>
                routeHandlerOptions.ThrowOnBadRequest = throwOnBadRequest);

        if (options.AddJsonStringEnumConverter)
            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(jsonOptions =>
            {
                jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        if (options.AddValidation)
            builder.Services.AddValidation();

        if (options.AddServiceDefaults)
            builder.AddServiceDefaults();

        return builder;
    }
}