using Matterway.Catalog.Api.Infrastructure.ImportExport;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Extensions;

var apiContract = ApiDirectory.Catalog;

var builder = BuilderBootstrap.CreateBuilder(args);

builder.Services.AddScoped<ICatalogArchiveService, CatalogArchiveService>();

builder
    .ConfigureApiFoundation(apiContract)
    .ConfigureAuthentication()
    .ConfigureImageStorage()
    .ConfigurePersistence()
    .ConfigureFeatures();

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();