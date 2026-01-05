using System.Text.Json.Serialization;
using FastEndpoints;
using Matterway.Customers.Api.Application;
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
    .ConfigureOpenApi()
    .ConfigureCors();

builder
    .ConfigureAuthentication()
    .ConfigureCatalogIntegration()
    .ConfigurePersistence()
    .ConfigureFastEndpoints();

var app = builder.Build();

app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.UseStatusCodePages();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpointsWithDefaults();

if (app.Environment.IsDevelopment())
{
    app.ApplyOpenApi();
    app.ApplyScalar();
}

app.Run();