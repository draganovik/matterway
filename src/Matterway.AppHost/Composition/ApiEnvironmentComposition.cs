namespace Matterway.AppHost.Composition;

internal static class ApiEnvironmentComposition
{
    public static void ConfigureApiEnvironment(
        IEnumerable<IResourceBuilder<ProjectResource>> apis,
        EndpointReference identityApiHttpEndpoint,
        WebEndpoints web,
        IResourceBuilder<ParameterResource> jwtSigningKey)
    {
        foreach (var api in apis)
            api
                .WithEnvironment("Jwt__Key", jwtSigningKey)
                .WithEnvironment("Jwt__Issuer", identityApiHttpEndpoint)
                .WithEnvironment("Jwt__Audience", identityApiHttpEndpoint)
                .WithEnvironment("Cors__AllowedOrigins__0", web.StorefrontHttpEndpoint)
                .WithEnvironment("Cors__AllowedOrigins__1", web.DashboardHttpEndpoint);
    }
}
