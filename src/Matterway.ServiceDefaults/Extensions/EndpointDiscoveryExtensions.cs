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
            var endpointAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var uniqueTypes = new HashSet<Type>();
            var serviceDescriptors = GetDefinedTypesSafe(endpointAssembly)
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
}