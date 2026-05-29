using System.ComponentModel.DataAnnotations;
using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.System.Orders.Contracts;

public record SystemCreateOrderRequest
{
    public EOrderType? Type { get; init; }

    public DeliveryInfoRequest? DeliveryInfo { get; init; }

    public record DeliveryInfoRequest
    {
        public string? Country { get; init; }

        [Required]
        public required string City { get; init; }

        [Required]
        public required string ZipCode { get; init; }

        [Required]
        public required string AddressLine1 { get; init; }

        public string? AddressLine2 { get; init; }

        public string? ContactPhone { get; init; }
    }
}