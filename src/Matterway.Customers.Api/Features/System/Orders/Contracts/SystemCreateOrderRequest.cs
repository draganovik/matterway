using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.System.Orders.Contracts;

public record SystemCreateOrderRequest
{
    [Required]
    public Guid CustomerId { get; init; }

    [Required]
    public OrderId OrderId { get; init; }

    public DeliveryInfoRequest? DeliveryInfo { get; init; }

    public record DeliveryInfoRequest
    {
        [Required]
        public string Country { get; init; } = string.Empty;

        [Required]
        public string City { get; init; } = string.Empty;

        [Required]
        public string ZipCode { get; init; } = string.Empty;

        [Required]
        public string AddressLine1 { get; init; } = string.Empty;

        public string? AddressLine2 { get; init; }

        public string? ContactPhone { get; init; }
    }
}