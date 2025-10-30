using System.Text.Json.Serialization;
using Catalog.Api.Infrastructure.Abstractions;
using Catalog.Api.Infrastructure.Persistence;
using Catalog.Api.Infrastructure.Repositories;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Services.Brokers;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Extensions;

public static class Extensions
{
    public static void ConfigureServices(this IHostApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"Properties/appsettings.{builder.Environment.EnvironmentName}.json", optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddDbContext<CatalogDb>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")
                                 ?? throw new InvalidOperationException(
                                     "Connection string 'CatalogDb' not found.")));

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        builder.Services.AddHttpClient<IIdentityServiceBroker, IdentityServiceBroker>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = configuration.ResolveServiceUri("identity-api", "Services:Identity:Url");
        });
        builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
        builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
    }
}