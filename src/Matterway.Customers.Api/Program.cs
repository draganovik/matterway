using Asp.Versioning;
using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Brokers.Identity;
using Matterway.Customers.Api.Infrastructure.Persistence;
using Matterway.ServiceDefaults.Api;

const string serviceName = "customers";
const string scalarTitle = "Matterway Customers API";
ApiVersion[] supportedApiVersions = [new(1, 0)];

var builder = ApiTemplateRegistration.CreateApiBuilder(args);

builder
    .ConfigureApiFoundation(serviceName, supportedApiVersions)
    .ConfigureAuthentication(new ApiAuthenticationFeatureOptions())
    .ConfigureCatalogIntegration()
    .ConfigureIdentityIntegration()
    .ConfigurePersistence()
    .ConfigureFeatures(new ApiFeatureDiscoveryOptions());

var app = builder.Build();

app.UseApiFoundation();
app.ApplyDevelopmentApiDocs(scalarTitle, supportedApiVersions);
app.ApplyEndpoints(supportedApiVersions);

app.Run();