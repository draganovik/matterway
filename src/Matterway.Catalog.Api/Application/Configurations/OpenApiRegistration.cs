using Asp.Versioning;
using Matterway.ServiceDefaults.Versioning;

namespace Matterway.Catalog.Api.Application.Configurations;

public static class OpenApiRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureOpenApi(IReadOnlyCollection<ApiVersion> supportedApiVersions)
        {
            foreach (var version in ApiVersioningConventions.NormalizeSupportedVersions(supportedApiVersions))
            {
                var documentName = ApiVersioningConventions.ToDocumentName(version);
                builder.Services.AddOpenApi(documentName, options =>
                {
                    options.ShouldInclude = description =>
                        ApiVersioningConventions.ShouldIncludeInDocument(description, version);
                    ApiVersioningConventions.AddBearerSecurity(options);
                });
            }

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