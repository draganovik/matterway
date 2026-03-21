using Microsoft.Extensions.Configuration;

namespace Matterway.AppHost.Composition;

internal static class ApiEnvironmentComposition
{
    public static void ConfigureApiEnvironment(
        IEnumerable<IResourceBuilder<ProjectResource>> apis,
        EndpointReference identityApiHttpEndpoint,
        IResourceBuilder<ParameterResource> jwtSigningKey,
        IResourceBuilder<ParameterResource> systemAccessKey,
        IConfiguration configuration,
        IEnumerable<string>? fallbackCorsOrigins = null)
    {
        var corsAllowedOrigins = configuration
                                     .GetSection("Cors:AllowedOrigins")
                                     .Get<string[]>()?
                                     .Where(origin => !string.IsNullOrWhiteSpace(origin))
                                     .ToArray();

        if (corsAllowedOrigins is null or { Length: 0 })
        {
            corsAllowedOrigins = fallbackCorsOrigins?
                                     .Where(origin => !string.IsNullOrWhiteSpace(origin))
                                     .ToArray()
                                 ?? [];
        }

        foreach (var api in apis)
        {
            api
                .WithEnvironment("Jwt__Key", jwtSigningKey)
                .WithEnvironment("Jwt__Issuer", identityApiHttpEndpoint)
                .WithEnvironment("Jwt__Audience", identityApiHttpEndpoint)
                .WithEnvironment("Security__SystemAccessKey", systemAccessKey);

            for (var i = 0; i < corsAllowedOrigins.Length; i++)
                api.WithEnvironment($"Cors__AllowedOrigins__{i}", corsAllowedOrigins[i]);
        }
    }
}
