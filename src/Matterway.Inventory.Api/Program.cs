using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Matterway.Common.Mapping;
using Matterway.Common.Services.Brokers;
using Matterway.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<ValidationProfile>(); },
    AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

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
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Get the system user ID from the token
            if (!Guid.TryParse(context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
                context.Fail("Unauthorized");

            // Get the token from the context
            var token = (context.SecurityToken as JwtSecurityToken)?.RawData;

            if (token == null) context.Fail("Unauthorized");

            // Get the session from the database based on the user ID and token
            // TODO: make a call to the Identity API to validate the token

            var identityServiceBroker =
                context.HttpContext.RequestServices.GetRequiredService<IIdentityServiceBroker>();
            var claimsPrincipal = await identityServiceBroker.ValidateTokenAsync(token!);
            if (claimsPrincipal == null) context.Fail("Unauthorized");
        }
    };
});
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient<IIdentityServiceBroker, IdentityServiceBroker>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = configuration.ResolveServiceUri("identity-api", "Services:Identity:Url");
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.Run();