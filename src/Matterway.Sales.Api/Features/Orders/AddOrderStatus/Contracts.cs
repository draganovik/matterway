using System.ComponentModel.DataAnnotations;
using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.Orders.AddOrderStatus;

public sealed record AddOrderStatusRequest
{
    [Required]
    public EOrderStatusType Status { get; init; }

    public string? Note { get; init; }
}

public sealed record OrderStatusResponse
{
    public EOrderStatusType Status { get; init; }
    public DateTime ChangedAt { get; init; }
    public string? Note { get; init; }
}