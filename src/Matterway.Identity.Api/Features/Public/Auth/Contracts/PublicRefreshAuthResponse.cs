namespace Matterway.Identity.Api.Features.Public.Auth.Contracts;

public class PublicRefreshAuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
    public DateTime RefreshExpires { get; set; }
}
