using System.ComponentModel.DataAnnotations;

namespace Matterway.Identity.Api.Features.Public.Auth.Contracts;

public class PublicRefreshAuthRequest
{
    [Required(ErrorMessage = "RefreshToken is required.")]
    public string? RefreshToken { get; set; }
}
