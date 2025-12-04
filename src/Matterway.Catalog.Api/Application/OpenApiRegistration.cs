using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;

namespace Matterway.Catalog.Api.Application;

public static class OpenApiRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureOpenApi()
        {
            builder.Services.AddOpenApi(options =>
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
                    var bearerReference = new OpenApiSecuritySchemeReference("Bearer", null, null);
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        { bearerReference, [] }
                    });

                    return Task.CompletedTask;
                });
            });

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication ApplyOpenApi()
        {
            app.MapOpenApi("/openapi/{documentName}.yaml");
            return app;
        }
    }
}