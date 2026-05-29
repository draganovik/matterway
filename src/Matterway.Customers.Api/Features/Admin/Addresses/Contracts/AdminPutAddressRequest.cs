using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Admin.Addresses.Contracts;

public record AdminPutAddressRequest
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
