using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalar.AspNetCore;

namespace Matterway.Catalog.Api.Infrastructure;

public static class EndpointRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureApiVersioning()
        {
            builder.Services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
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
        public WebApplication ApplyEndpoints()
        {
            var versionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var apiGroup = app.MapGroup("/api")
                .DisableAntiforgery();

            var versionedApiGroup = apiGroup
                .MapGroup("/v{version:apiVersion}")
                .WithApiVersionSet(versionSet);

            var endpoints = app.Services
                .GetRequiredService<IEnumerable<IEndpoint>>();

            foreach (var endpoint in endpoints) endpoint.MapEndpoint(versionedApiGroup);

            return app;
        }

        public WebApplication ApplyScalar()
        {
            app.MapScalarApiReference("/", options =>
            {
                options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
                options.WithTitle("Matterway Catalog API");
            });

            return app;
        }
    }
}