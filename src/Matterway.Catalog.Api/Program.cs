using System.Text.Json.Serialization;
using Matterway.Catalog.Api.Extensions;
using Matterway.Common.Extensions;
using Matterway.ServiceDefaults;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

builder.AddServiceDefaults();

// Configure environment-specific settings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Properties/appsettings.{builder.Environment.EnvironmentName}.json", optional: true,
        reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure JSON options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configure CORS
builder.ConfigureCors();

// Configure services
builder.ConfigurePersistence();
builder.ConfigureProxyServices();
builder.ConfigureAuthentication();
builder.ConfigureImageStorage();

// Add validation and problem details
builder.ConfigureProblemDetails();
builder.Services.AddValidation();
//builder.Services.Configure<RouteHandlerOptions>(o => { o.ThrowOnBadRequest = false; });

// Register endpoints and API explorer
builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();

// Add OpenAPI and API versioning
builder.Services.ConfigureOpenApi();
builder.Services.ConfigureApiVersioning();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();
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