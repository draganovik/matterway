using Matterway.Sales.Api.Domain.Entities;

namespace Matterway.Sales.Api.Features.Orders.AddOrderStatus;

public static class AddOrderStatusMapper
{
    public static OrderStatusResponse MapToResponse(OrderStatus entity)
    {
        return new OrderStatusResponse
        {
            Status = entity.Status,
            ChangedAt = entity.ChangedAt,
            Note = entity.Note
        };
    }
}