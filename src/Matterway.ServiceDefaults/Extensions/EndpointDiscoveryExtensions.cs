using System.Reflection;
using Matterway.ServiceDefaults.Bootstraps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Matterway.ServiceDefaults.Extensions;

public static class EndpointDiscoveryExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureEndpoints()
        {
            return builder.ConfigureEndpoints(new ApiEndpointDiscoveryOptions());
        }

        public IHostApplicationBuilder ConfigureEndpoints(ApiEndpointDiscoveryOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var uniqueTypes = new HashSet<Type>();
            var serviceDescriptors = ResolveAssemblies(options)
                .SelectMany(GetDefinedTypesSafe)
                .Where(type =>
                    type is { IsAbstract: false, IsInterface: false } &&
                    type.IsAssignableTo(typeof(IEndpoint)) &&
                    uniqueTypes.Add(type.AsType()))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type.AsType()))
                .ToArray();

            builder.Services.TryAddEnumerable(serviceDescriptors);

            return builder;
        }
    }

    private static IEnumerable<TypeInfo> GetDefinedTypesSafe(Assembly assembly)
    {
        try
        {
            return assembly.DefinedTypes;
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.OfType<TypeInfo>();
        }
        catch
        {
            return [];
        }
    }

    private static IReadOnlyCollection<Assembly> ResolveAssemblies(ApiEndpointDiscoveryOptions options)
    {
        var assemblies = new List<Assembly>();

        if (options.IncludeEntryAssembly && Assembly.GetEntryAssembly() is { } entryAssembly)
            assemblies.Add(entryAssembly);

        if (options.AdditionalAssemblies is { Count: > 0 })
            assemblies.AddRange(options.AdditionalAssemblies);

        if (assemblies.Count == 0)
            assemblies.Add(Assembly.GetExecutingAssembly());

        return assemblies.Distinct().ToArray();
    }
}