using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Self.Customers.Contracts;

public record SelfBaseCustomerResponse
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