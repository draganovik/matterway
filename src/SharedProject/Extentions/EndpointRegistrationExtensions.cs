using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.Models;

public static class EndpointRegistrationExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services)
    {
        return services.AddEndpoints(typeof(IEndpoint).Assembly);
    }

    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var uniqueTypes = new HashSet<Type>();

        var serviceDescriptors = assemblies
            .Where(assembly => assembly is not null)
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)) &&
                           uniqueTypes.Add(type.AsType()))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type.AsType()))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services
            .GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder =
            routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}
