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
    false);

var jwtConfigurationSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtConfigurationSection.Get<JwtTokenOptions>() ?? new JwtTokenOptions();
builder.Services.Configure<JwtTokenOptions>(jwtConfigurationSection);
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder
    .ConfigureApi(
        apiContract,
        false)
    .ConfigureAuthentication(options =>
    {
        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
        options.TokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier;
        options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(Math.Max(0, jwtOptions.ClockSkewMinutes));
        options.Events = new JwtBearerEvents { OnTokenValidated = ValidateAccessTokenAsync };
    })
    .ConfigureIdentity()
    .ConfigurePersistence()
    .ConfigureEndpoints();

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();

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