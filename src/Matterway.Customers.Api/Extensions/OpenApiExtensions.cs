using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;

namespace Matterway.Customers.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT authorization header using the Bearer scheme."
                };

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, _) =>
            {
                var isAnonymous = context.Description.ActionDescriptor
                    .EndpointMetadata.OfType<AllowAnonymousAttribute>()
                    .Any();

                if (isAnonymous) return Task.CompletedTask;

                operation.Security ??= new List<OpenApiSecurityRequirement>();

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication ApplyOpenApi(this WebApplication app)
    {
        app.MapOpenApi("/openapi/{documentName}.yaml");
        return app;
    }
}