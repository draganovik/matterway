using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Self.Addresses.Contracts;

public record SelfBaseAddressResponse
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    public string? Country { get; init; }

    [Required]
    public string? City { get; init; }

    [Required]
    public string? ZipCode { get; init; }

    [Required]
    public string? AddressLine1 { get; init; }

    [Required]
    public string? AddressLine2 { get; init; }

    [Required]
    public string? ContactPhone { get; init; }
}
