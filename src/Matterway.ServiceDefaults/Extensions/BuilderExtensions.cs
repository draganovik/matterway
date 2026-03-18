using System.Text;
using Asp.Versioning;
using Matterway.ServiceDefaults.Authorization;
using Matterway.ServiceDefaults.Bootstraps;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.ServiceDefaults.Extensions;

public static class BuilderExtensions
{
    private const string CorsAllowedOriginsConfigurationPath = "Cors:AllowedOrigins";
    private const string CorsDiscoveryServiceNamesConfigurationPath = "Cors:DiscoveryServiceNames";

    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureApi(
            ApiDefinition apiDefinition,
            bool customizeBadHttpRequestProblemDetails = true,
            Action<ProblemDetailsContext>? customizeProblemDetails = null,
            Action<CorsPolicyBuilder>? configureCorsPolicy = null)
        {
            ArgumentNullException.ThrowIfNull(apiDefinition);

            ConfigureRequestIdentity(builder, apiDefinition.ServiceName);
            ConfigureSystemAccessKey(builder);
            ConfigureProblemDetails(builder, customizeBadHttpRequestProblemDetails, customizeProblemDetails);
            ConfigureApiVersioning(builder, apiDefinition.SupportedApiVersions);
            ConfigureOpenApi(builder, apiDefinition.SupportedApiVersions);
            ConfigureCors(builder, configureCorsPolicy);

            return builder;
        }

        public IHostApplicationBuilder ConfigureAuthentication(ApiAuthenticationOptions? options = null)
        {
            options ??= new ApiAuthenticationOptions();
            ArgumentNullException.ThrowIfNull(options);

            options.ConfigureServices?.Invoke(builder);

            var validIssuer = options.ValidIssuer ?? builder.Configuration["Jwt:Issuer"];
            var validAudience = options.ValidAudience ?? builder.Configuration["Jwt:Audience"];
            if (string.IsNullOrWhiteSpace(validIssuer))
                throw new InvalidOperationException("JWT issuer not configured.");
            if (string.IsNullOrWhiteSpace(validAudience))
                throw new InvalidOperationException("JWT audience not configured.");

            builder.Services.AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    var signingKey = builder.Configuration[options.SigningKeyConfigurationPath]
                                     ?? throw new InvalidOperationException("JWT signing key not configured.");

                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = validIssuer,
                        ValidAudience = validAudience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                    };

                    options.ConfigureTokenValidation?.Invoke(tokenValidationParameters, builder);

                    jwtOptions.RequireHttpsMetadata = false;
                    jwtOptions.SaveToken = true;
                    jwtOptions.TokenValidationParameters = tokenValidationParameters;
                    options.ConfigureJwtBearer?.Invoke(jwtOptions, builder);
                });

            builder.Services.AddAuthorization();
            return builder;
        }
    }

    private static void ConfigureRequestIdentity(IHostApplicationBuilder builder, string serviceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        builder.Services.AddSingleton<IOptions<RequestIdentityOptions>>(_ => Options.Create(new RequestIdentityOptions
        {
            ServiceName = serviceName
        }));
    }

    private static void ConfigureSystemAccessKey(IHostApplicationBuilder builder)
    {
        var accessKey = builder.Configuration[SystemAccessKeyOptions.ConfigurationPath];
        if (string.IsNullOrWhiteSpace(accessKey))
            throw new InvalidOperationException(
                $"System access key not configured. Set '{SystemAccessKeyOptions.ConfigurationPath}'.");

        var allowedAuthorities = ResolveInternalApiAuthorities(builder.Configuration);

        builder.Services.AddSingleton<IOptions<SystemAccessKeyOptions>>(_ => Options.Create(new SystemAccessKeyOptions
        {
            AccessKey = accessKey,
            AllowedAuthorities = allowedAuthorities
        }));
    }

    private static IReadOnlySet<string> ResolveInternalApiAuthorities(IConfiguration configuration)
    {
        var configuredAuthorities = ApiDirectory.All
            .Select(api => configuration[api.AccessOriginConfigurationPath])
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Select(static value =>
            {
                return Uri.TryCreate(value, UriKind.Absolute, out var uri)
                    ? uri.Authority
                    : null;
            })
            .Where(static authority => !string.IsNullOrWhiteSpace(authority))
            .Select(static authority => authority!);

        var serviceDiscoveryAuthorities = ApiDirectory.All
            .Select(static api => api.AspireServiceName);

        return configuredAuthorities
            .Concat(serviceDiscoveryAuthorities)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static void ConfigureCors(
        IHostApplicationBuilder builder,
        Action<CorsPolicyBuilder>? configureCorsPolicy)
    {
        var configuredOrigins = builder.Configuration
                                    .GetSection(CorsAllowedOriginsConfigurationPath)
                                    .Get<string[]>()
                                ?? [];

        var discoveryServiceNames = builder.Configuration
                                        .GetSection(CorsDiscoveryServiceNamesConfigurationPath)
                                        .Get<string[]>()
                                    ?? [];

        var discoveredOrigins = discoveryServiceNames
            .Select(serviceName => builder.Configuration.ResolveServiceUri(serviceName)
                .GetLeftPart(UriPartial.Authority));

        var allowedOrigins = configuredOrigins
            .Concat(discoveredOrigins)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        builder.Services.AddCors(corsOptions =>
        {
            corsOptions.AddDefaultPolicy(policy =>
            {
                if (configureCorsPolicy is not null)
                {
                    configureCorsPolicy(policy);
                    return;
                }

                if (allowedOrigins.Length == 0)
                    return;

                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }

    private static void ConfigureProblemDetails(
        IHostApplicationBuilder builder,
        bool customizeBadHttpRequestProblemDetails,
        Action<ProblemDetailsContext>? customizeProblemDetails)
    {
        builder.Services.AddProblemDetails(problemDetailsOptions =>
        {
            problemDetailsOptions.CustomizeProblemDetails = context =>
            {
                if (customizeBadHttpRequestProblemDetails)
                    context.ApplyBadRequestProblemDetails();

                customizeProblemDetails?.Invoke(context);
            };
        });
    }

    private static void ConfigureApiVersioning(
        IHostApplicationBuilder builder,
        IReadOnlyCollection<ApiVersion> supportedApiVersions)
    {
        var versions = ApiVersioningConventions.NormalizeSupportedVersions(supportedApiVersions);

        builder.Services.AddApiVersioning(versioningOptions =>
            {
                versioningOptions.DefaultApiVersion = versions[0];
                versioningOptions.AssumeDefaultVersionWhenUnspecified = true;
                versioningOptions.ReportApiVersions = true;
                versioningOptions.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(explorerOptions =>
            {
                explorerOptions.GroupNameFormat = "'v'VVV";
                explorerOptions.SubstituteApiVersionInUrl = true;
            });
    }

    private static void ConfigureOpenApi(
        IHostApplicationBuilder builder,
        IReadOnlyCollection<ApiVersion> supportedApiVersions)
    {
        var versions = ApiVersioningConventions.NormalizeSupportedVersions(supportedApiVersions);

        foreach (var version in versions)
        {
            var documentName = ApiVersioningConventions.ToDocumentName(version);
            builder.Services.AddOpenApi(documentName, openApiOptions =>
            {
                openApiOptions.ShouldInclude =
                    description => ApiVersioningConventions.ShouldIncludeInDocument(description, version);
                ApiVersioningConventions.AddBearerSecurity(openApiOptions);
            });
        }
    }
}