using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Payments.API.Data;
using Payments.API.Endpoints;
using Payments.API.Repository;
using Shared.ServiceBrokers;
using SharedProject.Profiles;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentsDbContext") ?? throw new InvalidOperationException("Connection string 'PaymentsDbContext' not found.")));
builder.Services
    .AddControllers(setup => setup.ReturnHttpNotAcceptable = true)
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddScoped<IIdentityServiceBroker, IdentityServiceBroker>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ValidationProfile>();
}, AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Description = "Please insert JWT token into field"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Get the system user ID from the token
            if (!Guid.TryParse(context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            {
                context.Fail("Unauthorized");
            }

            // Get the token from the context
            var token = (context.SecurityToken as JwtSecurityToken)?.RawData;

            if (token == null)
            {
                context.Fail("Unauthorized");
            }

            // Get the session from the database based on the user ID and token
            // TODO: make a call to the Identity API to validate the token

            var identityServiceBroker = context.HttpContext.RequestServices.GetRequiredService<IIdentityServiceBroker>();
            var claimsPrincipal = await identityServiceBroker.ValidateTokenAsync(token!);
            if (claimsPrincipal == null)
            {
                context.Fail("Unauthorized");
            }
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000", "http://localhost:3001")
               .AllowAnyMethod()
               .AllowAnyHeader().AllowCredentials();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPaymentEndpoints();

app.UseCors();

app.Run();