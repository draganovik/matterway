using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.ServiceDefaults.Bootstraps;

public sealed record ApiAuthenticationOptions
{
    public string SigningKeyConfigurationPath { get; init; } = "Jwt:Key";
    public string? ValidIssuer { get; init; }
    public string? ValidAudience { get; init; }
    public Action<IHostApplicationBuilder>? ConfigureServices { get; init; }
    public Action<TokenValidationParameters, IHostApplicationBuilder>? ConfigureTokenValidation { get; init; }
    public Action<JwtBearerOptions, IHostApplicationBuilder>? ConfigureJwtBearer { get; init; }
}