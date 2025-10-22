using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Features.Sessions.Contracts;

public class SessionBaseRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string? Password { get; set; }
}