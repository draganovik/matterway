using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Catalog.API.Data;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Shared.Models;
using Shared.ServiceBrokers;
using SharedProject.Profiles;

var builder = WebApplication.CreateBuilder(args);

ConfigureDatabase(builder.Services, builder.Configuration);
ConfigureRepositories(builder.Services);
ConfigureStringEnums(builder.Services);
ConfigureMapper(builder.Services);
ConfigureAuthentication(builder.Services, builder.Configuration);
ConfigureCors(builder.Services);
builder.Services.AddProblemDetails();
ConfigureOpenApi(builder.Services);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

ConfigurePipeline(app);
app.MapEndpoints();

app.Run();

return;

static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration) =>
    services.AddDbContext<CatalogDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("CatalogDbContext")
                             ?? throw new InvalidOperationException(
                                 "Connection string 'CatalogDbContext' not found.")));

static void ConfigureRepositories(IServiceCollection services)
{
    services.AddScoped<IIdentityServiceBroker, IdentityServiceBroker>();
    services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
    services.AddScoped<IProductImageRepository, ProductImageRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
}

static void ConfigureStringEnums(IServiceCollection services) =>
    services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

static void ConfigureMapper(IServiceCollection services) =>
    services.AddAutoMapper(cfg => cfg.AddProfile<ValidationProfile>(), AppDomain.CurrentDomain.GetAssemblies());

static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    services
        .AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            var signingKey = configuration["Jwt:Key"]
                             ?? throw new InvalidOperationException("JWT signing key not configured.");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var identifier = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!Guid.TryParse(identifier, out _))
                    {
                        context.Fail("Unauthorized");
                        return;
                    }

                    if (context.SecurityToken is not JwtSecurityToken jwt || string.IsNullOrWhiteSpace(jwt.RawData))
                    {
                        context.Fail("Unauthorized");
                        return;
                    }

                    var broker = context.HttpContext.RequestServices.GetRequiredService<IIdentityServiceBroker>();
                    if (await broker.ValidateTokenAsync(jwt.RawData) is null)
                    {
                        context.Fail("Unauthorized");
                    }
                }
            };
        });

    services.AddAuthorization();
}

static void ConfigureCors(IServiceCollection services) =>
    services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

static void ConfigureOpenApi(IServiceCollection services) =>
    services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT authorization header using the Bearer scheme."
            };

            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, _) =>
        {
            var isAnonymous = context.Description.ActionDescriptor
                .EndpointMetadata.OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>()
                .Any();

            if (isAnonymous) return Task.CompletedTask;

            operation.Security ??= new List<OpenApiSecurityRequirement>();

            return Task.CompletedTask;
        });
    });

static void ConfigurePipeline(WebApplication app)
{
    app.UseExceptionHandler();
    app.UseStatusCodePages();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    if (!app.Environment.IsDevelopment()) return;
    app.MapOpenApi("/openapi/{documentName}.yaml");

    app.MapScalarApiReference("/", options =>
    {
        options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
        options.WithTitle("Matterway Catalog API");
    });
}
