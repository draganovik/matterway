using Identity.API.Data;
using Identity.API.Endpoints;
using Identity.API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDbContext") ?? throw new InvalidOperationException("Connection string 'IdentityDbContext' not found.")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPasswordHasher<SystemUser>, PasswordHasher<SystemUser>>();

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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

            if (token == null) context.Fail("Unauthorized");

            // Get the session from the database based on the user ID and token
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<IdentityDbContext>();
            var session = await dbContext.Session.FirstOrDefaultAsync(s => s.SystemUserId == systemUserId && s.Token == token);

            if (session == null)
            {
                context.Fail("Unauthorized");
            }
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapSystemUserEndpoints();

app.MapSessionEndpoints();

app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (BadHttpRequestException ex)
    {
        ctx.Response.StatusCode = ex.StatusCode;
        await ctx.Response.WriteAsync(ex.Message);
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.Run();
