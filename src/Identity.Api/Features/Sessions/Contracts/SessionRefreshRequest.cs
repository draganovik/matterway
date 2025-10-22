using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Features.Sessions.Contracts;

public class SessionRefreshRequest
{
    [Required]
    public string? RefreshToken { get; set; }

    public string? TokenType { get; set; } = "Bearer";
}