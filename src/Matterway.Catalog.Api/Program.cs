using System.Text.Json.Serialization;
using Matterway.Catalog.Api.Infrastructure;
using Matterway.Catalog.Api.Providers.Persistence;
using Matterway.Catalog.Api.Providers.Storage;
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

builder
    .ConfigureProblemDetails()
    .ConfigureApiVersioning()
    .ConfigureOpenApi()
    .ConfigureCors();

builder
    .ConfigureAuthentication()
    .ConfigureImageStorage()
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
    app.ApplyScalar();
}

app.ApplyEndpoints();

app.Run();