using System.Reflection;
using Matterway.ServiceDefaults.Bootstraps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Matterway.ServiceDefaults.Extensions;

public static class FeatureDiscoveryExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureFeatures()
        {
            return builder.ConfigureFeatures(new ApiFeatureDiscoveryOptions());
        }

        public IHostApplicationBuilder ConfigureFeatures(ApiFeatureDiscoveryOptions options)
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
        catch
        {
            return [];
        }
    }

    private static IReadOnlyCollection<Assembly> ResolveAssemblies(ApiFeatureDiscoveryOptions options)
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