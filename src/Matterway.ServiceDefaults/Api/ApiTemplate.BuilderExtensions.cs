using System.Text;
using Asp.Versioning;
using Matterway.ServiceDefaults.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.ServiceDefaults.Api;

public static partial class ApiTemplate
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureApiFoundation(
            ApiContract apiContract,
            ApiProblemDetailsFeatureOptions? problemDetailsOptions = null,
            ApiCorsFeatureOptions? corsOptions = null)
        {
            ArgumentNullException.ThrowIfNull(apiContract);

            return builder.ConfigureApiFoundation(
                apiContract.ServiceName,
                apiContract.SupportedApiVersions,
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

        public IHostApplicationBuilder ConfigureAuthentication(ApiAuthenticationFeatureOptions? options = null)
        {
            options ??= new ApiAuthenticationFeatureOptions();
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
            RequestIdentity.Configure(options);
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
                        ApplyBadRequestProblemDetails(context);

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

        public IHostApplicationBuilder ConfigureFeatures(ApiFeatureDiscoveryOptions? options = null)
        {
            var featureOptions = options ?? new ApiFeatureDiscoveryOptions();
            var uniqueTypes = new HashSet<Type>();
            var assemblies = featureOptions.Assemblies ?? AppDomain.CurrentDomain.GetAssemblies();

            var serviceDescriptors = assemblies
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.DefinedTypes;
                    }
                    catch
                    {
                        return [];
                    }
                })
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
}