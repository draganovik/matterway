using Asp.Versioning;
using System.Text.Json.Serialization;
using Matterway.Customers.Api.Application.Configurations;
using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Persistence;
using Matterway.Customers.Api.Infrastructure.Brokers.Identity;
using Matterway.ServiceDefaults;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Properties/appsettings.{builder.Environment.EnvironmentName}.json", true,
        true)
    .AddEnvironmentVariables();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddValidation();
builder.AddServiceDefaults();

ApiVersion[] supportedApiVersions = [new(1, 0)];

builder
    .ConfigureProblemDetails()
    .ConfigureApiVersioning(supportedApiVersions)
    .ConfigureOpenApi(supportedApiVersions)
    .ConfigureCors();

builder
    .ConfigureAuthentication()
    .ConfigureCatalogIntegration()
    .ConfigureIdentityIntegration()
    .ConfigurePersistence()
    .ConfigureFeatures();

var app = builder.Build();

app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.UseStatusCodePages();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.ApplyOpenApi();
    app.ApplyScalar(supportedApiVersions);
}

app.ApplyEndpoints(supportedApiVersions);

app.Run();