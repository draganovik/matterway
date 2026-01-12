using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.Identity.Api.Providers.Services.AuthToken;

public class JwtTokenService : ITokenService
{
    public const string TokenUseClaim = "token_use";
    public const string SecurityStampClaim = "sst";
    public const string RefreshTokenIdClaim = "rti";
    public const string AccessTokenUse = "access";
    public const string RefreshTokenUse = "refresh";
    internal const string RefreshTokenIdProvider = "Matterway.Identity.Api";
    internal const string RefreshTokenIdName = "refresh_token_id";

    private readonly UserManager<SystemUser> _userManager;
    private readonly JwtTokenOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(UserManager<SystemUser> userManager, IOptions<JwtTokenOptions> options)
    {
        _userManager = userManager;
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(_options.Key))
            throw new InvalidOperationException("JWT signing key not configured.");

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
    }

    public async Task<AuthTokenServiceResponse> CreateTokenPairAsync(SystemUser user,
        CancellationToken cancellationToken = default)
    {
        var refreshTokenId = await RotateRefreshTokenIdAsync(user);
        if (string.IsNullOrWhiteSpace(refreshTokenId))
            throw new InvalidOperationException("Failed to issue refresh token.");

        return await CreateTokenPairWithRefreshIdAsync(user, refreshTokenId, cancellationToken);
    }

    public async Task<AuthTokenServiceResponse?> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var principal = ValidateToken(refreshToken, true);
        if (principal is null) return null;

        var tokenUse = principal.FindFirstValue(TokenUseClaim);
        if (!string.Equals(tokenUse, RefreshTokenUse, StringComparison.Ordinal)) return null;

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(userId, out var parsedUserId)) return null;

        var user = await _userManager.FindByIdAsync(parsedUserId.ToString());
        if (user is null) return null;

        if (await _userManager.IsLockedOutAsync(user)) return null;

        var securityStamp = await _userManager.GetSecurityStampAsync(user);
        var tokenSecurityStamp = principal.FindFirstValue(SecurityStampClaim);
        if (string.IsNullOrWhiteSpace(tokenSecurityStamp)) return null;
        if (!string.Equals(securityStamp, tokenSecurityStamp, StringComparison.Ordinal)) return null;

        var tokenRefreshTokenId = principal.FindFirstValue(RefreshTokenIdClaim);
        if (string.IsNullOrWhiteSpace(tokenRefreshTokenId)) return null;

        var storedRefreshTokenId = await _userManager.GetAuthenticationTokenAsync(user,
            RefreshTokenIdProvider, RefreshTokenIdName);
        if (!string.Equals(storedRefreshTokenId, tokenRefreshTokenId, StringComparison.Ordinal)) return null;

        var refreshTokenId = await RotateRefreshTokenIdAsync(user);
        if (string.IsNullOrWhiteSpace(refreshTokenId)) return null;

        return await CreateTokenPairWithRefreshIdAsync(user, refreshTokenId, cancellationToken);
    }

    private async Task<AuthTokenServiceResponse> CreateTokenPairWithRefreshIdAsync(
        SystemUser user,
        string refreshTokenId,
        CancellationToken cancellationToken)
    {
        var issuedAt = DateTime.UtcNow;
        var accessToken = await CreateTokenAsync(user, AccessTokenUse,
            TimeSpan.FromMinutes(_options.AccessTokenMinutes), true, issuedAt, null, cancellationToken);
        var refreshToken = await CreateTokenAsync(user, RefreshTokenUse,
            TimeSpan.FromDays(_options.RefreshTokenDays), false, issuedAt, refreshTokenId, cancellationToken);

        return new AuthTokenServiceResponse
        {
            Token = accessToken.Token,
            RefreshToken = refreshToken.Token,
            Created = issuedAt,
            Expires = accessToken.Expires,
            RefreshExpires = refreshToken.Expires
        };
    }

    private async Task<(string Token, DateTime Expires)> CreateTokenAsync(
        SystemUser user,
        string tokenUse,
        TimeSpan lifetime,
        bool includeUserClaims,
        DateTime issuedAt,
        string? refreshTokenId,
        CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(TokenUseClaim, tokenUse)
        };

        var securityStamp = await _userManager.GetSecurityStampAsync(user);
        if (!string.IsNullOrWhiteSpace(securityStamp))
            claims.Add(new Claim(SecurityStampClaim, securityStamp));

        if (string.Equals(tokenUse, RefreshTokenUse, StringComparison.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(refreshTokenId))
                throw new InvalidOperationException("Refresh token id is missing.");

            claims.Add(new Claim(RefreshTokenIdClaim, refreshTokenId));
        }

        if (includeUserClaims)
        {
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = issuedAt.Add(lifetime),
            Issuer = _options.Issuer,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
        };

        var token = _tokenHandler.CreateToken(descriptor);
        return (_tokenHandler.WriteToken(token), token.ValidTo);
    }

    private async Task<string?> RotateRefreshTokenIdAsync(SystemUser user)
    {
        if (string.IsNullOrWhiteSpace(user.UserName))
            user.UserName = string.IsNullOrWhiteSpace(user.Email) ? user.Id.ToString() : user.Email;

        if (string.IsNullOrWhiteSpace(user.NormalizedUserName))
            user.NormalizedUserName = _userManager.NormalizeName(user.UserName);

        var refreshTokenId = Guid.NewGuid().ToString("N");
        var result = await _userManager.SetAuthenticationTokenAsync(user,
            RefreshTokenIdProvider, RefreshTokenIdName, refreshTokenId);
        return result.Succeeded ? refreshTokenId : null;
    }

    private ClaimsPrincipal? ValidateToken(string token, bool validateLifetime)
    {
        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = !string.IsNullOrWhiteSpace(_options.Issuer),
                ValidIssuer = _options.Issuer,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.FromMinutes(Math.Max(0, _options.ClockSkewMinutes))
            };

            return _tokenHandler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }
}