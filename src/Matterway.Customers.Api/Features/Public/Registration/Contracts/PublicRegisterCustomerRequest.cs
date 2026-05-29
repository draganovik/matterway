using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Public.Registration.Contracts;

public record PublicRegisterCustomerRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    [MinLength(6)]
    public required string Password { get; init; }

    [Required]
    public required string FirstName { get; init; }

    [Required]
    public required string LastName { get; init; }

    [Required]
    public DateOnly BirthDate { get; init; }
}
