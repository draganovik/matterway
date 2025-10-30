using Matterway.Catalog.Api.Extensions;
using Matterway.Common.Extensions;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

builder.AddServiceDefaults();

// Configure services
builder.ConfigureServices();
builder.ConfigureAuthentication();

// Add validation and problem details
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.Configure<RouteHandlerOptions>(o => { o.ThrowOnBadRequest = false; });

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