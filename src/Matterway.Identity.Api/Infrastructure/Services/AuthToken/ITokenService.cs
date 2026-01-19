using Matterway.Identity.Api.Domain.Entities;

namespace Matterway.Identity.Api.Infrastructure.Services.AuthToken;

public interface ITokenService
{
    Task<AuthTokenServiceResponse> CreateTokenPairAsync(SystemUser user,
        CancellationToken cancellationToken = default);

    Task<AuthTokenServiceResponse?> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default);
}

public class AuthTokenServiceResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
    public DateTime RefreshExpires { get; set; }
}