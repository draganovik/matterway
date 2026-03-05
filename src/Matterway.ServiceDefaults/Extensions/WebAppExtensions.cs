using Asp.Versioning;
using Matterway.ServiceDefaults.Authorization;
using Matterway.ServiceDefaults.Bootstraps;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Matterway.ServiceDefaults.Extensions;

public static class WebAppExtensions
{
    private const string ApiRoutePrefix = "/api";

    extension(WebApplication app)
    {
        public WebApplication UseApiFoundation()
        {
            app.UseExceptionHandler();
            app.MapDefaultEndpoints();
            app.UseStatusCodePages();
            app.Use(async (httpContext, next) =>
            {
                var options = httpContext.RequestServices.GetRequiredService<IOptions<RequestIdentityOptions>>().Value;
                using var _ = RequestIdentity.PushOptions(options);
                await next();
            });
            app.Use(async (httpContext, next) =>
            {
                var requiresSystemAccessKey =
                    SystemAccessKeyEndpointExtensions.IsSystemAccessKeyRequired(httpContext.GetEndpoint()?.Metadata);

                if (!requiresSystemAccessKey)
                {
                    await next();
                    return;
                }

                var options = httpContext.RequestServices.GetRequiredService<IOptions<SystemAccessKeyOptions>>().Value;
                if (!httpContext.Request.Headers.TryGetValue(SystemAccessKeyOptions.HeaderName, out var providedKey) ||
                    string.IsNullOrWhiteSpace(providedKey) ||
                    !string.Equals(providedKey.ToString().Trim(), options.AccessKey, StringComparison.Ordinal))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                await next();
            });
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        public WebApplication ApplyApiContract(ApiDefinition apiDefinition)
        {
            ArgumentNullException.ThrowIfNull(apiDefinition);

            if (app.Environment.IsDevelopment())
                app.MapOpenApi(ApiDocumentationDefaults.OpenApiRoutePattern);

            ApplyEndpoints(app, apiDefinition.SupportedApiVersions);

            return app;
        }
    }

    private static void ApplyEndpoints(WebApplication app, IReadOnlyCollection<ApiVersion> supportedApiVersions)
    {
        var versions = ApiVersioningConventions.NormalizeSupportedVersions(supportedApiVersions);
        var versionSetBuilder = app.NewApiVersionSet()
            .ReportApiVersions();

        foreach (var version in versions)
            versionSetBuilder.HasApiVersion(version);

        var versionSet = versionSetBuilder.Build();

        var apiGroup = app.MapGroup(ApiRoutePrefix)
            .DisableAntiforgery();

        RouteGroupBuilder MapKindGroup(EndpointKind endpointKind)
        {
            var endpointSegment = endpointKind.ToString().ToLowerInvariant();

            return apiGroup.MapGroup($"/{endpointSegment}")
                .MapGroup("/v{version:apiVersion}")
                .WithApiVersionSet(versionSet);
        }

        var endpointGroups = Enum.GetValues<EndpointKind>()
            .ToDictionary(endpointKind => endpointKind, MapKindGroup);

        var endpointRouter = new EndpointRouter(endpointGroups);

        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(endpointRouter);
    }
}