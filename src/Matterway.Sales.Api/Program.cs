using Asp.Versioning;
using Matterway.Sales.Api.Infrastructure.Brokers.Customers;
using Matterway.Sales.Api.Infrastructure.Persistence;

const string serviceName = "sales";
const string scalarTitle = "Matterway Sales API";
ApiVersion[] supportedApiVersions = [new(1, 0)];

var builder = ApiTemplateRegistration.CreateApiBuilder(args);

builder
    .ConfigureApiFoundation(serviceName, supportedApiVersions)
    .ConfigureAuthentication(new ApiAuthenticationFeatureOptions())
    .ConfigureCustomersIntegration()
    .ConfigurePersistence()
    .ConfigureFeatures(new ApiFeatureDiscoveryOptions());

var app = builder.Build();

app.UseApiFoundation();
app.ApplyDevelopmentApiDocs(scalarTitle, supportedApiVersions);
app.ApplyEndpoints(supportedApiVersions);

app.Run();