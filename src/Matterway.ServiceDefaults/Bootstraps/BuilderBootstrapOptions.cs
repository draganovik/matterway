using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.ServiceDefaults.Bootstraps;

public sealed record BuilderBootstrapOptions
{
    public string? ContentRootPath { get; init; }
    public string EnvironmentSettingsFilePattern { get; init; } = "Properties/appsettings.{0}.json";
    public bool EnvironmentSettingsOptional { get; init; } = true;
    public bool EnvironmentSettingsReloadOnChange { get; init; } = true;
    public bool AddEnvironmentVariables { get; init; } = true;
    public bool AddJsonStringEnumConverter { get; init; } = true;
    public bool AddValidation { get; init; } = true;
    public bool AddServiceDefaults { get; init; } = true;
    public bool? ThrowOnBadRequest { get; init; }
}

public sealed record ApiVersioningFeatureOptions
{
    public required IReadOnlyCollection<ApiVersion> SupportedApiVersions { get; init; }

    public bool AssumeDefaultVersionWhenUnspecified { get; init; } = true;
    public bool ReportApiVersions { get; init; } = true;
    public IApiVersionReader ApiVersionReader { get; init; } = new UrlSegmentApiVersionReader();
    public string GroupNameFormat { get; init; } = "'v'VVV";
    public bool SubstituteApiVersionInUrl { get; init; } = true;
}

public sealed record ApiOpenApiFeatureOptions
{
    public required IReadOnlyCollection<ApiVersion> SupportedApiVersions { get; init; }
    public bool EnableBearerSecurity { get; init; } = true;
}

public sealed record ApiCorsFeatureOptions
{
    public string AllowedOriginsConfigurationPath { get; init; } = "Cors:AllowedOrigins";
    public string[] FallbackAllowedOrigins { get; init; } = ["http://localhost:3000", "http://localhost:3001"];
    public string[] DiscoveryServiceNames { get; init; } = ["storefront-web", "dashboard-web"];
    public Action<CorsPolicyBuilder>? ConfigurePolicy { get; init; }
}

public sealed record ApiProblemDetailsFeatureOptions
{
    public bool EnableBadHttpRequestCustomization { get; init; } = true;
    public Action<ProblemDetailsContext>? CustomizeProblemDetails { get; init; }
}

public sealed record ApiAuthenticationFeatureOptions
{
    public string SigningKeyConfigurationPath { get; init; } = "Jwt:Key";
    public bool RequireHttpsMetadata { get; init; } = false;
    public bool SaveToken { get; init; } = true;
    public bool ValidateIssuer { get; init; } = false;
    public bool ValidateAudience { get; init; } = false;
    public string? ValidIssuer { get; init; }
    public string? ValidAudience { get; init; }
    public bool ValidateLifetime { get; init; } = true;
    public bool ValidateIssuerSigningKey { get; init; } = true;
    public Action<IHostApplicationBuilder>? ConfigureServices { get; init; }
    public Action<TokenValidationParameters, IHostApplicationBuilder>? ConfigureTokenValidation { get; init; }
    public Action<JwtBearerOptions, IHostApplicationBuilder>? ConfigureJwtBearer { get; init; }
}

public sealed record ApiFeatureDiscoveryOptions
{
    public bool IncludeEntryAssembly { get; init; } = true;
    public IReadOnlyCollection<Assembly>? AdditionalAssemblies { get; init; }
}

public sealed record ApiEndpointRoutingFeatureOptions
{
    public required IReadOnlyCollection<ApiVersion> SupportedApiVersions { get; init; }
    public EndpointKind[] EndpointKinds { get; init; } = Enum.GetValues<EndpointKind>();
    public string ApiRoutePrefix { get; init; } = "/api";
    public bool DisableAntiforgery { get; init; } = true;
    public Action<ApiVersionSetBuilder>? ConfigureVersionSet { get; init; }
}

public sealed record ApiOpenApiRouteOptions
{
    public string OpenApiRoutePattern { get; init; } = ApiDocumentationDefaults.OpenApiRoutePattern;
}

public static class ApiDocumentationDefaults
{
    public const string DefaultDocumentName = "v1";
    public const string OpenApiRoutePattern = "/openapi/{documentName}.yaml";
}