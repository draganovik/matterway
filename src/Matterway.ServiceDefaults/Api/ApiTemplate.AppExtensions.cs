using Asp.Versioning;
using Matterway.ServiceDefaults.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Matterway.ServiceDefaults.Api;

public static partial class ApiTemplate
{
    extension(WebApplication app)
    {
        public WebApplication UseApiFoundation()
        {
            app.UseExceptionHandler();
            app.MapDefaultEndpoints();
            app.UseStatusCodePages();
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
            ApiContract apiContract,
            ApiOpenApiRouteOptions? openApiRouteOptions = null)
        {
            ArgumentNullException.ThrowIfNull(apiContract);

            app.ApplyDevelopmentApiDocs(openApiRouteOptions);
            app.ApplyEndpoints(apiContract);

            return app;
        }

        public WebApplication ApplyEndpoints(ApiEndpointRoutingFeatureOptions options)
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
            return app.ApplyEndpoints(new ApiEndpointRoutingFeatureOptions
            {
                SupportedApiVersions = supportedApiVersions
            });
        }

        public WebApplication ApplyEndpoints(ApiContract apiContract)
        {
            ArgumentNullException.ThrowIfNull(apiContract);
            return app.ApplyEndpoints(apiContract.SupportedApiVersions);
        }
    }
}