using Matterway.Customers.Api.Extensions;
using Matterway.ServiceDefaults;

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
builder.Services.Configure<RouteHandlerOptions>(o => { o.ThrowOnBadRequest = false; });

builder.ConfigureAuthentication();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRepositories();
builder.Services.ConfigureFeatures();

builder.Services.ConfigureMapper();
builder.Services.ConfigureJsonOptions();

builder.Services.ConfigureOpenApi();
builder.Services.ConfigureApiVersioning();
//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddValidation();
builder.ConfigureCors();

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