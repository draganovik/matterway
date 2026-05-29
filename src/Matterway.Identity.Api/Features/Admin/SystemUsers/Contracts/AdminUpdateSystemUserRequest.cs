using System.ComponentModel.DataAnnotations;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers.Contracts;

public class AdminUpdateSystemUserRequest
{
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string? Password { get; set; }
}