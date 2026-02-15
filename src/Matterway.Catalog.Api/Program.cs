using Asp.Versioning;
using Matterway.Catalog.Api.Infrastructure.ImportExport;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Matterway.ServiceDefaults.Api;

const string serviceName = "catalog";
const string scalarTitle = "Matterway Catalog API";
ApiVersion[] supportedApiVersions = [new(1, 0)];

var builder = ApiTemplateRegistration.CreateApiBuilder(args);

builder.Services.AddScoped<ICatalogArchiveService, CatalogArchiveService>();

builder
    .ConfigureApiFoundation(serviceName, supportedApiVersions)
    .ConfigureAuthentication(new ApiAuthenticationFeatureOptions())
    .ConfigureImageStorage()
    .ConfigurePersistence()
    .ConfigureFeatures(new ApiFeatureDiscoveryOptions());

var app = builder.Build();

app.UseApiFoundation();
app.ApplyDevelopmentApiDocs(scalarTitle, supportedApiVersions);
app.ApplyEndpoints(supportedApiVersions);

app.Run();