using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Admin.Orders.Contracts;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Features.Admin.Orders.Endpoints;

public class AdminGetOrderById : IEndpoint
{
    private const string RouteName = nameof(AdminGetOrderById);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "orders/{orderId:OrderId}", Handler)
            .WithName(RouteName).WithSummary("[admin] Get Order by id")
            .WithTags(nameof(Order))
            .Produces<AdminBaseOrderResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminBaseOrderResponse>, NotFound>> Handler(
        OrderId orderId,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        var entity = await orderRepository.GetById(orderId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(ToResponse(entity));
    }

    private static decimal CalculateTotal(IEnumerable<OrderItem> items)
    {
        var total = items.Sum(item => item.UnitPrice * item.Quantity);
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    private static AdminBaseOrderResponse ToResponse(Order entity)
    {
        return new AdminBaseOrderResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Type = entity.Type,
            TotalAmount = CalculateTotal(entity.Items),
            PlacedAt = entity.PlacedAt,
            DeliveryInfo = entity.DeliveryInfo is null
                ? null
                : new AdminBaseOrderResponse.DeliveryInfoResponse
                {
                    Country = entity.DeliveryInfo.Country,
                    City = entity.DeliveryInfo.City,
                    ZipCode = entity.DeliveryInfo.ZipCode,
                    AddressLine1 = entity.DeliveryInfo.AddressLine1,
                    AddressLine2 = entity.DeliveryInfo.AddressLine2,
                    ContactPhone = entity.DeliveryInfo.ContactPhone
                },
            Items = entity.Items.Select(item => new AdminBaseOrderResponse.ItemResponse
            {
                Id = item.Id,
                ArticleCode = ArticleCode.Parse(item.ArticleCode, null),
                ArticleTitle = item.ArticleTitle,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList(),
            StatusHistory = entity.StatusHistory.Select(status => new AdminBaseOrderResponse.StatusResponse
            {
                Status = status.Status,
                ChangedAt = status.ChangedAt,
                Note = status.Note
            }).ToList(),
            Payments = entity.Payments.Select(payment => new AdminBaseOrderResponse.PaymentSnapshotResponse
            {
                Id = payment.Id,
                Provider = payment.Provider,
                Amount = payment.Amount,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            }).ToList()
        };
    }
}
