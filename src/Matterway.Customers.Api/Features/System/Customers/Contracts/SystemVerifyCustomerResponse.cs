using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.System.Customers.Contracts;

public record SystemVerifyCustomerResponse
{
    [Required]
    public Guid SystemUserId { get; init; }

    [Required]
    public string? FirstName { get; init; }

    [Required]
    public string? LastName { get; init; }

    [Required]
    public DateOnly BirthDate { get; init; }

    public Guid? DefaultAddressId { get; init; }
}
