namespace Matterway.Identity.Api.Infrastructure.Services.AuthToken;

public sealed class JwtTokenOptions
{
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public string Key { get; init; } = string.Empty;
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 14;
    public int ClockSkewMinutes { get; init; } = 1;
}