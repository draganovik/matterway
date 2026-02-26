using System.Text;
using Asp.Versioning;
using Matterway.ServiceDefaults.Authorization;
using Matterway.ServiceDefaults.Bootstraps;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.ServiceDefaults.Extensions;

public static class BuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureApiFoundation(
            ApiDefinition apiDefinition,
            ApiProblemDetailsFeatureOptions? problemDetailsOptions = null,
            ApiCorsFeatureOptions? corsOptions = null)
        {
            ArgumentNullException.ThrowIfNull(apiDefinition);

            return builder.ConfigureApiFoundation(
                apiDefinition.ServiceName,
                apiDefinition.SupportedApiVersions,
                problemDetailsOptions,
                corsOptions);
        }

        public IHostApplicationBuilder ConfigureApiFoundation(
            string serviceName,
            IReadOnlyCollection<ApiVersion> supportedApiVersions,
            ApiProblemDetailsFeatureOptions? problemDetailsOptions = null,
            ApiCorsFeatureOptions? corsOptions = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
            ArgumentNullException.ThrowIfNull(supportedApiVersions);

            return builder
                .ConfigureRequestIdentity(new RequestIdentityOptions
                {
                    ServiceName = serviceName
                })
                .ConfigureProblemDetails(problemDetailsOptions ?? new ApiProblemDetailsFeatureOptions())
                .ConfigureApiVersioning(new ApiVersioningFeatureOptions
                {
                    SupportedApiVersions = supportedApiVersions
                })
                .ConfigureOpenApi(new ApiOpenApiFeatureOptions
                {
                    SupportedApiVersions = supportedApiVersions
                })
                .ConfigureCors(corsOptions ?? new ApiCorsFeatureOptions());
        }

        public IHostApplicationBuilder ConfigureAuthentication()
        {
            return builder.ConfigureAuthentication(new ApiAuthenticationFeatureOptions());
        }

        public IHostApplicationBuilder ConfigureAuthentication(ApiAuthenticationFeatureOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            options.ConfigureServices?.Invoke(builder);

            builder.Services.AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.RequireHttpsMetadata = options.RequireHttpsMetadata;
                    jwtOptions.SaveToken = options.SaveToken;

                    var signingKey = builder.Configuration[options.SigningKeyConfigurationPath]
                                     ?? throw new InvalidOperationException("JWT signing key not configured.");

                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = options.ValidateIssuer,
                        ValidateAudience = options.ValidateAudience,
                        ValidIssuer = options.ValidIssuer,
                        ValidAudience = options.ValidAudience,
                        ValidateLifetime = options.ValidateLifetime,
                        ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                    };

                    options.ConfigureTokenValidation?.Invoke(tokenValidationParameters, builder);

                    jwtOptions.TokenValidationParameters = tokenValidationParameters;
                    options.ConfigureJwtBearer?.Invoke(jwtOptions, builder);
                });

            builder.Services.AddAuthorization();
            return builder;
        }

        public IHostApplicationBuilder ConfigureRequestIdentity(RequestIdentityOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ServiceName);

            builder.Services.AddSingleton<IOptions<RequestIdentityOptions>>(_ => Options.Create(options));
            return builder;
        }

        public IHostApplicationBuilder ConfigureCors(ApiCorsFeatureOptions? options = null)
        {
            options ??= new ApiCorsFeatureOptions();

            var configuredOrigins = builder.Configuration
                                        .GetSection(options.AllowedOriginsConfigurationPath)
                                        .Get<string[]>()
                                    ?? options.FallbackAllowedOrigins;

            var discoveredOrigins = options.DiscoveryServiceNames
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
                    if (options.ConfigurePolicy is not null)
                    {
                        options.ConfigurePolicy(policy);
                        return;
                    }

                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            return builder;
        }

        public IHostApplicationBuilder ConfigureProblemDetails(ApiProblemDetailsFeatureOptions? options = null)
        {
            options ??= new ApiProblemDetailsFeatureOptions();

            builder.Services.AddProblemDetails(problemDetailsOptions =>
            {
                problemDetailsOptions.CustomizeProblemDetails = context =>
                {
                    if (options.EnableBadHttpRequestCustomization)
                        context.ApplyBadRequestProblemDetails();

                    options.CustomizeProblemDetails?.Invoke(context);
                };
            });

            return builder;
        }

        public IHostApplicationBuilder ConfigureApiVersioning(ApiVersioningFeatureOptions options)
        {
            var versions = ApiVersioningConventions.NormalizeSupportedVersions(options.SupportedApiVersions);

            builder.Services.AddApiVersioning(versioningOptions =>
                {
                    versioningOptions.DefaultApiVersion = versions[0];
                    versioningOptions.AssumeDefaultVersionWhenUnspecified =
                        options.AssumeDefaultVersionWhenUnspecified;
                    versioningOptions.ReportApiVersions = options.ReportApiVersions;
                    versioningOptions.ApiVersionReader = options.ApiVersionReader;
                })
                .AddApiExplorer(explorerOptions =>
                {
                    explorerOptions.GroupNameFormat = options.GroupNameFormat;
                    explorerOptions.SubstituteApiVersionInUrl = options.SubstituteApiVersionInUrl;
                });

            return builder;
        }

        public IHostApplicationBuilder ConfigureOpenApi(ApiOpenApiFeatureOptions options)
        {
            var versions = ApiVersioningConventions.NormalizeSupportedVersions(options.SupportedApiVersions);

            foreach (var version in versions)
            {
                var documentName = ApiVersioningConventions.ToDocumentName(version);
                builder.Services.AddOpenApi(documentName, openApiOptions =>
                {
                    openApiOptions.ShouldInclude =
                        description => ApiVersioningConventions.ShouldIncludeInDocument(description, version);

                    if (options.EnableBearerSecurity)
                        ApiVersioningConventions.AddBearerSecurity(openApiOptions);
                });
            }

            return builder;
        }
    }
}