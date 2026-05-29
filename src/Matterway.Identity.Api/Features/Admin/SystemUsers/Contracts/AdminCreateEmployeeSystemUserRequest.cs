using System.ComponentModel.DataAnnotations;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers.Contracts;

public class AdminCreateEmployeeSystemUserRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public required string Password { get; init; }
}