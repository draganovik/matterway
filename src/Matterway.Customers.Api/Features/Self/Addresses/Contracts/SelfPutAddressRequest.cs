using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Self.Addresses.Contracts;

public record SelfPutAddressRequest
{
    [Required]
    public string Country { get; init; } = string.Empty;

    [Required]
    public string City { get; init; } = string.Empty;

    [Required]
    public string ZipCode { get; init; } = string.Empty;

    [Required]
    public string AddressLine1 { get; init; } = string.Empty;

    [Required]
    public string AddressLine2 { get; init; } = string.Empty;

    [Required]
    public string ContactPhone { get; init; } = string.Empty;
}
