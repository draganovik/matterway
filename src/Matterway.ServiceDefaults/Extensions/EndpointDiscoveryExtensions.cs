using System.Reflection;
using Matterway.ServiceDefaults.Bootstraps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Matterway.ServiceDefaults.Extensions;

public static class EndpointDiscoveryExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureEndpoints()
        {
            var endpointAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            foreach (var type in endpointAssembly.DefinedTypes.Where(type =>
                         type is { IsAbstract: false, IsInterface: false } &&
                         type.IsAssignableTo(typeof(IEndpoint))))
                builder.Services.AddTransient(typeof(IEndpoint), type.AsType());

            return builder;
        }
    }
}