using System.ComponentModel.DataAnnotations;

namespace Matterway.Identity.Api.Features.Public.Auth.Contracts;

public class PublicLoginAuthRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string? Password { get; set; }
}
