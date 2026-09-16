using Asp.Versioning;
using Matterway.ServiceDefaults.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Matterway.ServiceDefaults.Extensions;

public static class BuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureApi(
            ApiDefinition apiDefinition,
            bool customizeBadHttpRequestProblemDetails = true)
        {
            ArgumentNullException.ThrowIfNull(apiDefinition);

            ConfigureRequestIdentity(builder, apiDefinition.ServiceName);
            ConfigureSystemAccessKey(builder);
            builder.Services.AddProblemDetails(options =>
            {
                if (customizeBadHttpRequestProblemDetails)
                    options.CustomizeProblemDetails = context => context.ApplyBadRequestProblemDetails();
            });

            var versions = ApiVersioningConventions.NormalizeSupportedVersions(apiDefinition.SupportedApiVersions);
            ConfigureApiVersioning(builder, versions);
            ConfigureOpenApi(builder, versions);

            return builder;
        }
    }

    private static void ConfigureRequestIdentity(IHostApplicationBuilder builder, string serviceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        builder.Services.AddSingleton<IOptions<RequestIdentityOptions>>(_ => Options.Create(new RequestIdentityOptions
        {
            ServiceName = serviceName
        }));
    }

    private static void ConfigureSystemAccessKey(IHostApplicationBuilder builder)
    {
        var accessKey = builder.Configuration[SystemAccessKeyOptions.ConfigurationPath];
        if (string.IsNullOrWhiteSpace(accessKey))
            throw new InvalidOperationException(
                $"System access key not configured. Set '{SystemAccessKeyOptions.ConfigurationPath}'.");

        var allowedAuthorities = ResolveInternalApiAuthorities(builder.Configuration);

        builder.Services.AddSingleton<IOptions<SystemAccessKeyOptions>>(_ => Options.Create(new SystemAccessKeyOptions
        {
            AccessKey = accessKey,
            AllowedAuthorities = allowedAuthorities
        }));
    }

    private static IReadOnlySet<string> ResolveInternalApiAuthorities(IConfiguration configuration)
    {
        var configuredAuthorities = ApiDirectory.All
            .Select(api => configuration.ResolveServiceUri(api))
            .Select(static uri => uri.Authority);

        var serviceDiscoveryAuthorities = ApiDirectory.All
            .Select(static api => api.AspireServiceName);

        return configuredAuthorities
            .Concat(serviceDiscoveryAuthorities)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static void ConfigureApiVersioning(
        IHostApplicationBuilder builder,
        ApiVersion[] versions)
    {
        builder.Services.AddApiVersioning(versioningOptions =>
        {
            versioningOptions.DefaultApiVersion = versions[0];
            versioningOptions.AssumeDefaultVersionWhenUnspecified = true;
            versioningOptions.ReportApiVersions = true;
            versioningOptions.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
    }

    private static void ConfigureOpenApi(
        IHostApplicationBuilder builder,
        ApiVersion[] versions)
    {
        foreach (var version in versions)
        {
            var documentName = ApiVersioningConventions.ToDocumentName(version);
            builder.Services.AddOpenApi(documentName, openApiOptions =>
            {
                openApiOptions.ShouldInclude =
                    description => OpenApiConventions.ShouldIncludeInDocument(
                        description.ActionDescriptor.EndpointMetadata,
                        version);
                OpenApiConventions.SubstituteRouteVersion(openApiOptions, version);
                OpenApiConventions.AddSecurity(openApiOptions);
            });
        }
    }
}