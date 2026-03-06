namespace Matterway.Sales.Api.Domain.Entities;

public class OrderDeliveryInfo
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public OrderId OrderId { get; set; }
    public Order? Order { get; set; }

    public required string Country { get; set; } = string.Empty;
    public required string City { get; set; } = string.Empty;
    public required string ZipCode { get; set; } = string.Empty;
    public required string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string? ContactPhone { get; set; }
}