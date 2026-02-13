using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Identity.Api.Application.Configurations;
using Matterway.Identity.Api.Infrastructure.Persistence;
using Matterway.ServiceDefaults;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

builder.AddServiceDefaults();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Properties/appsettings.{builder.Environment.EnvironmentName}.json", true,
        true)
    .AddEnvironmentVariables();

builder.Services.AddProblemDetails();
builder.Services.Configure<RouteHandlerOptions>(options => { options.ThrowOnBadRequest = false; });

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddValidation();
ApiVersion[] supportedApiVersions = [new(1, 0)];

builder
    .ConfigureAuthentication()
    .ConfigureIdentity()
    .ConfigurePersistence()
    .ConfigureFeatures();

builder
    .ConfigureOpenApi(supportedApiVersions)
    .ConfigureApiVersioning(supportedApiVersions)
    .ConfigureCors();

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
    app.ApplyScalar(supportedApiVersions);
}

app.ApplyEndpoints(supportedApiVersions);

app.Run();