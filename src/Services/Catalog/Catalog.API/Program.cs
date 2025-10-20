using Catalog.API.Configurations;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Problem Details middleware
builder.Services.AddProblemDetails();

// Add Authentication, Database, and Repositories
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRepositories();

// Add Mapping and JSON options
builder.Services.ConfigureMapper();
builder.Services.ConfigureJsonOptions();

// Register endpoints and API explorer
builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();

// Add OpenAPI and API versioning
builder.Services.ConfigureOpenApi();
builder.Services.ConfigureApiVersioning();

// Add CORS policy
builder.Services.ConfigureCors();

var app = builder.Build();

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