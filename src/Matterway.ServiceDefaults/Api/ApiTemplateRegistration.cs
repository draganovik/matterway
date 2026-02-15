using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.ServiceDefaults.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace Matterway.ServiceDefaults.Api;

public static class ApiTemplateRegistration
{
    public static WebApplicationBuilder CreateApiBuilder(
        string[] args,
        ApiBootstrapFeatureOptions? options = null)
    {
        options ??= new ApiBootstrapFeatureOptions();
        var contentRootPath = options.ContentRootPath ?? Directory.GetCurrentDirectory();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = contentRootPath
        });

        builder.Configuration
            .SetBasePath(contentRootPath)
            .AddJsonFile(
                string.Format(options.EnvironmentSettingsFilePattern, builder.Environment.EnvironmentName),
                options.EnvironmentSettingsOptional,
                options.EnvironmentSettingsReloadOnChange);

        if (options.AddEnvironmentVariables)
            builder.Configuration.AddEnvironmentVariables();

        if (options.ThrowOnBadRequest is bool throwOnBadRequest)
            builder.Services.Configure<RouteHandlerOptions>(routeHandlerOptions =>
                routeHandlerOptions.ThrowOnBadRequest = throwOnBadRequest);

        if (options.AddJsonStringEnumConverter)
            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(jsonOptions =>
            {
                jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        if (options.AddValidation)
            builder.Services.AddValidation();

        if (options.AddServiceDefaults)
            builder.AddServiceDefaults();

        return builder;
    }

    extension(IHostApplicationBuilder builder)
    {
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

    extension(WebApplication app)
    {
        public WebApplication UseApiFoundation()
        {
            app.UseExceptionHandler();
            app.MapDefaultEndpoints();
            app.UseStatusCodePages();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        public WebApplication ApplyDevelopmentApiDocs(
            string title,
            IReadOnlyCollection<ApiVersion> supportedApiVersions,
            ApiOpenApiRouteOptions? openApiRouteOptions = null)
        {
            if (!app.Environment.IsDevelopment())
                return app;

            app.ApplyOpenApi(openApiRouteOptions ?? new ApiOpenApiRouteOptions());
            app.ApplyScalar(new ApiScalarFeatureOptions
            {
                SupportedApiVersions = supportedApiVersions,
                Title = title
            });

            return app;
        }

        public WebApplication ApplyOpenApi(ApiOpenApiRouteOptions? options = null)
        {
            var routeOptions = options ?? new ApiOpenApiRouteOptions();
            app.MapOpenApi(routeOptions.OpenApiRoutePattern);
            return app;
        }

        public WebApplication ApplyScalar(ApiScalarFeatureOptions options)
        {
            var documentNames = ApiVersioningConventions.ToDocumentNames(options.SupportedApiVersions);

            app.MapScalarApiReference(options.RoutePrefix, (scalarOptions, _) =>
            {
                scalarOptions.WithOpenApiRoutePattern(options.OpenApiRoutePattern);
                scalarOptions.WithTitle(options.Title);
                scalarOptions.AddDocuments(documentNames);
            });

            return app;
        }

        public WebApplication ApplyEndpoints(ApiEndpointRoutingFeatureOptions options)
        {
            var versions = ApiVersioningConventions.NormalizeSupportedVersions(options.SupportedApiVersions);
            var versionSetBuilder = app.NewApiVersionSet()
                .ReportApiVersions();

            foreach (var version in versions)
                versionSetBuilder.HasApiVersion(version);

            options.ConfigureVersionSet?.Invoke(versionSetBuilder);

            var versionSet = versionSetBuilder.Build();

            var apiGroup = app.MapGroup(options.ApiRoutePrefix);
            if (options.DisableAntiforgery)
                apiGroup = apiGroup.DisableAntiforgery();

            RouteGroupBuilder MapKindGroup(EndpointKind endpointKind)
            {
                var endpointSegment = endpointKind.ToString().ToLowerInvariant();

                return apiGroup.MapGroup($"/{endpointSegment}")
                    .MapGroup("/v{version:apiVersion}")
                    .WithApiVersionSet(versionSet);
            }

            var endpointGroups = options.EndpointKinds
                .Distinct()
                .ToDictionary(endpointKind => endpointKind, MapKindGroup);

            var endpointRouter = new EndpointRouter(endpointGroups);

            var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
            foreach (var endpoint in endpoints) endpoint.MapEndpoint(endpointRouter);

            return app;
        }

        public WebApplication ApplyEndpoints(IReadOnlyCollection<ApiVersion> supportedApiVersions)
        {
            return app.ApplyEndpoints(new ApiEndpointRoutingFeatureOptions
            {
                SupportedApiVersions = supportedApiVersions
            });
        }
    }

    private static void ApplyBadRequestProblemDetails(ProblemDetailsContext context)
    {
        if (context.Exception is not BadHttpRequestException
            {
                InnerException: JsonException jsonException
            })
        {
            if (context.Exception is BadHttpRequestException badHttpException)
            {
                context.HttpContext.Response.StatusCode = badHttpException.StatusCode;
                context.ProblemDetails = new ProblemDetails
                {
                    Status = badHttpException.StatusCode,
                    Title = "Bad Request",
                    Detail = badHttpException.Message,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
                };
                context.ProblemDetails.Extensions["traceId"] =
                    Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
            }

            return;
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        var member = jsonException.Path?.TrimStart('$').TrimStart('.');
        if (!string.IsNullOrWhiteSpace(member))
            context.ProblemDetails = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [member] =
                [
                    $"Value provided for '{member}' has an invalid format. Please check the value and try again."
                ]
            })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid request payload format.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };
        else
            context.ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid JSON payload.",
                Detail = "The request body contains malformed JSON. Please fix the payload and try again.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };

        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
    }
}