using Matterway.Catalog.Api.Infrastructure.ImportExport;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Storage;

var apiContract = ApiContracts.Catalog;

var builder = ApiTemplate.CreateApiBuilder(args);

builder.Services.AddScoped<ICatalogArchiveService, CatalogArchiveService>();

builder
    .ConfigureApiFoundation(apiContract)
    .ConfigureAuthentication(new ApiAuthenticationFeatureOptions())
    .ConfigureImageStorage()
    .ConfigurePersistence()
    .ConfigureFeatures(new ApiFeatureDiscoveryOptions());

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();