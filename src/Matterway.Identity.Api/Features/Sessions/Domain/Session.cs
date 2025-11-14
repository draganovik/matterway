using System.ComponentModel.DataAnnotations;
using Matterway.Identity.Api.Features.SystemUsers.Domain;

namespace Matterway.Identity.Api.Features.Sessions.Domain;

public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "SystemUserId is required.")]
    public Guid? SystemUserId { get; set; }

    public SystemUser? SystemUser { get; set; }

    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "RefreshToken is required.")]
    public string RefreshToken { get; set; } = string.Empty;

    public DateTime? Created { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Expires is required.")]
    public DateTime? Expires { get; set; }

    [Required(ErrorMessage = "Refresh expires is required.")]
    public DateTime? RefreshExpires { get; set; }

    public bool IsExpired()
    {
        return Expires <= DateTime.UtcNow;
    }

    public bool IsRefreshExpired()
    {
        return RefreshExpires <= DateTime.UtcNow;
    }
}