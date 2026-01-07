namespace Matterway.Customers.Api.Domain.Entities;

public class Address
{
    public Guid Id { get; set; }
    public required string Country { get; set; } = string.Empty;
    public required string City { get; set; } = string.Empty;
    public required string ZipCode { get; set; } = string.Empty;
    public required string AddressLine1 { get; set; } = string.Empty;
    public required string AddressLine2 { get; set; } = string.Empty;
    public required string ContactPhone { get; set; } = string.Empty;

    public required Guid CustomerId { get; set; }
}