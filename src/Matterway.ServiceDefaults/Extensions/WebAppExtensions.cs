using Asp.Versioning;
using Matterway.ServiceDefaults.Authorization;
using Matterway.ServiceDefaults.Bootstraps;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Matterway.ServiceDefaults.Extensions;

public static class WebAppExtensions
{
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
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        public WebApplication ApplyDevelopmentApiDocs(
            ApiOpenApiRouteOptions? openApiRouteOptions = null)
        {
            if (!app.Environment.IsDevelopment())
                return app;

            app.ApplyOpenApi(openApiRouteOptions ?? new ApiOpenApiRouteOptions());

            return app;
        }

        public WebApplication ApplyOpenApi(ApiOpenApiRouteOptions? options = null)
        {
            var routeOptions = options ?? new ApiOpenApiRouteOptions();
            app.MapOpenApi(routeOptions.OpenApiRoutePattern);
            return app;
        }

        public WebApplication ApplyApiContract(
            ApiDefinition apiDefinition,
            ApiOpenApiRouteOptions? openApiRouteOptions = null)
        {
            ArgumentNullException.ThrowIfNull(apiDefinition);

            app.ApplyDevelopmentApiDocs(openApiRouteOptions);
            app.ApplyEndpoints(apiDefinition);

            return app;
        }

        public WebApplication ApplyEndpoints(ApiEndpointRoutingOptions options)
        {
            var versions = ApiVersioningConventions.NormalizeSupportedVersions(options.SupportedApiVersions);
            var versionSetBuilder = app.NewApiVersionSet()
                .ReportApiVersions();

            foreach (var version in versions)
                versionSetBuilder.HasApiVersion(version);

            options.ConfigureVersionSet?.Invoke(versionSetBuilder);

            var versionSet = versionSetBuilder.Build();

            var apiGroup = app.MapGroup(options.ApiRoutePrefix);
            if (options.DisableAntiforgery)
                apiGroup = apiGroup.DisableAntiforgery();

            RouteGroupBuilder MapKindGroup(EndpointKind endpointKind)
            {
                var endpointSegment = endpointKind.ToString().ToLowerInvariant();

                return apiGroup.MapGroup($"/{endpointSegment}")
                    .MapGroup("/v{version:apiVersion}")
                    .WithApiVersionSet(versionSet);
            }

            var endpointGroups = options.EndpointKinds
                .Distinct()
                .ToDictionary(endpointKind => endpointKind, MapKindGroup);

            var endpointRouter = new EndpointRouter(endpointGroups);

            var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
            foreach (var endpoint in endpoints)
                endpoint.MapEndpoint(endpointRouter);

            return app;
        }

        public WebApplication ApplyEndpoints(IReadOnlyCollection<ApiVersion> supportedApiVersions)
        {
            return app.ApplyEndpoints(new ApiEndpointRoutingOptions
            {
                SupportedApiVersions = supportedApiVersions
            });
        }

        public WebApplication ApplyEndpoints(ApiDefinition apiDefinition)
        {
            ArgumentNullException.ThrowIfNull(apiDefinition);
            return app.ApplyEndpoints(apiDefinition.SupportedApiVersions);
        }
    }
}