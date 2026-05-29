using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Admin.Customers.Contracts;

public record AdminUpdateCustomerRequest
{
    [Required]
    public Guid SystemUserId { get; init; }

    [Required]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    public string LastName { get; init; } = string.Empty;

    [Required]
    public DateOnly BirthDate { get; init; }

    public Guid? DefaultAddressId { get; init; }
}