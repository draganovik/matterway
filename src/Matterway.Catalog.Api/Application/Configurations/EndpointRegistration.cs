using Asp.Versioning;
using Matterway.ServiceDefaults.Versioning;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalar.AspNetCore;

namespace Matterway.Catalog.Api.Application.Configurations;

public static class EndpointRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureApiVersioning(IReadOnlyCollection<ApiVersion> supportedApiVersions)
        {
            var versions = ApiVersioningConventions.NormalizeSupportedVersions(supportedApiVersions);

            builder.Services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = versions[0];
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            return builder;
        }

        public IHostApplicationBuilder ConfigureFeatures()
        {
            var uniqueTypes = new HashSet<Type>();

            var serviceDescriptors = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.DefinedTypes;
                    }
                    catch
                    {
                        return [];
                    } // avoid ReflectionTypeLoadException
                })
                .Where(type =>
                    type is { IsAbstract: false, IsInterface: false } &&
                    type.IsAssignableTo(typeof(IEndpoint)) &&
                    uniqueTypes.Add(type.AsType()))
                .Select(type =>
                    ServiceDescriptor.Transient(typeof(IEndpoint), type.AsType()))
                .ToArray();

            builder.Services.TryAddEnumerable(serviceDescriptors);

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication ApplyEndpoints(IReadOnlyCollection<ApiVersion> supportedApiVersions)
        {
            var versions = ApiVersioningConventions.NormalizeSupportedVersions(supportedApiVersions);

            var versionSetBuilder = app.NewApiVersionSet()
                .ReportApiVersions();

            foreach (var version in versions)
                versionSetBuilder.HasApiVersion(version);

            var versionSet = versionSetBuilder.Build();

            var apiGroup = app.MapGroup("/api")
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

            var endpoints = app.Services
                .GetRequiredService<IEnumerable<IEndpoint>>();

            foreach (var endpoint in endpoints) endpoint.MapEndpoint(endpointRouter);

            return app;
        }

        public WebApplication ApplyScalar(IReadOnlyCollection<ApiVersion> supportedApiVersions)
        {
            var apiDocumentNames = ApiVersioningConventions.ToDocumentNames(supportedApiVersions);

            app.MapScalarApiReference("/", (options, _) =>
            {
                options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
                options.WithTitle("Matterway Catalog API");
                options.AddDocuments(apiDocumentNames);
            });

            return app;
        }
    }
}