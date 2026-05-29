using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.Admin.Orders.Contracts;

public record AdminAddOrderStatusResponse
{
    public EOrderStatusType Status { get; init; }
    public DateTime ChangedAt { get; init; }
    public string? Note { get; init; }
}