using System.Security.Claims;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Infrastructure.Configurations;
using Matterway.Identity.Api.Infrastructure.Persistence;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;
using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

var apiContract = ApiDirectory.Identity;

var builder = BuilderBootstrap.CreateBuilder(
    args,
    new BuilderBootstrapOptions
    {
        ThrowOnBadRequest = false
    });

var jwtConfigurationSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtConfigurationSection.Get<JwtTokenOptions>() ?? new JwtTokenOptions();

builder
    .ConfigureApiFoundation(
        apiContract,
        new ApiProblemDetailsFeatureOptions
        {
            EnableBadHttpRequestCustomization = false
        })
    .ConfigureAuthentication(CreateAuthenticationOptions(jwtConfigurationSection, jwtOptions))
    .ConfigureIdentity()
    .ConfigurePersistence()
    .ConfigureFeatures();

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();

static ApiAuthenticationFeatureOptions CreateAuthenticationOptions(
    IConfigurationSection jwtConfigurationSection,
    JwtTokenOptions jwtOptions)
{
    return new ApiAuthenticationFeatureOptions
    {
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtOptions.Issuer),
        ValidIssuer = jwtOptions.Issuer,
        ConfigureServices = hostBuilder =>
        {
            hostBuilder.Services.Configure<JwtTokenOptions>(jwtConfigurationSection);
            hostBuilder.Services.AddScoped<ITokenService, JwtTokenService>();
        },
        ConfigureTokenValidation = (tokenValidationParameters, _) =>
        {
            tokenValidationParameters.RoleClaimType = ClaimTypes.Role;
            tokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier;
            tokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(Math.Max(0, jwtOptions.ClockSkewMinutes));
        },
        ConfigureJwtBearer = (jwtBearerOptions, _) =>
        {
            jwtBearerOptions.Events = new JwtBearerEvents
            {
                OnTokenValidated = ValidateAccessTokenAsync
            };
        }
    };
}

static async Task ValidateAccessTokenAsync(TokenValidatedContext context)
{
    var tokenUse = context.Principal?.FindFirstValue(JwtTokenService.TokenUseClaim);
    if (!string.Equals(tokenUse, JwtTokenService.AccessTokenUse, StringComparison.Ordinal))
    {
        context.Fail("Unauthorized");
        return;
    }

    var identifier = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(identifier, out var userId))
    {
        context.Fail("Unauthorized");
        return;
    }

    var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<SystemUser>>();
    var user = await userManager.FindByIdAsync(userId.ToString());
    if (user is null)
    {
        context.Fail("Unauthorized");
        return;
    }

    var securityStampClaim = context.Principal?.FindFirstValue(JwtTokenService.SecurityStampClaim);
    if (string.IsNullOrWhiteSpace(securityStampClaim))
    {
        context.Fail("Unauthorized");
        return;
    }

    var securityStamp = await userManager.GetSecurityStampAsync(user);
    if (!string.Equals(securityStamp, securityStampClaim, StringComparison.Ordinal))
        context.Fail("Unauthorized");
}