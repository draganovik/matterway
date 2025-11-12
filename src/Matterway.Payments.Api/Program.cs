using Matterway.Payments.Api.Extensions;
using Matterway.Common.Extensions;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

builder.AddServiceDefaults();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"Properties/appsettings.{builder.Environment.EnvironmentName}.json", optional: true,
        reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddProblemDetails();
builder.Services.Configure<RouteHandlerOptions>(o => { o.ThrowOnBadRequest = false; });

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRepositories();

builder.Services.ConfigureMapper();
builder.Services.ConfigureJsonOptions();

builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureOpenApi();
builder.Services.ConfigureApiVersioning();

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