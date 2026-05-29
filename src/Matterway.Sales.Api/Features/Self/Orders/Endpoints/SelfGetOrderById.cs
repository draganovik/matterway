using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Self.Orders.Contracts;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Features.Self.Orders.Endpoints;

public class SelfGetOrderById : IEndpoint
{
    private const string RouteName = nameof(SelfGetOrderById);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "orders/{orderId:OrderId}", Handler)
            .WithName(RouteName).WithSummary("[self] Get own Order by id.")
            .WithTags(nameof(Order))
            .Produces<SelfBaseOrderResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<SelfBaseOrderResponse>, NotFound, ForbidHttpResult>> Handler(
        OrderId orderId,
        HttpContext httpContext,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.Forbid();

        var entity = await orderRepository.GetById(orderId, cancellationToken);
        if (entity is null || entity.CustomerId != customerId) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(entity));
    }

    private static decimal CalculateTotal(IEnumerable<OrderItem> items)
    {
        var total = items.Sum(item => item.UnitPrice * item.Quantity);
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    private static SelfBaseOrderResponse ToResponse(Order entity)
    {
        return new SelfBaseOrderResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Type = entity.Type,
            TotalAmount = CalculateTotal(entity.Items),
            PlacedAt = entity.PlacedAt,
            DeliveryInfo = entity.DeliveryInfo is null
                ? null
                : new SelfBaseOrderResponse.DeliveryInfoResponse
                {
                    Country = entity.DeliveryInfo.Country,
                    City = entity.DeliveryInfo.City,
                    ZipCode = entity.DeliveryInfo.ZipCode,
                    AddressLine1 = entity.DeliveryInfo.AddressLine1,
                    AddressLine2 = entity.DeliveryInfo.AddressLine2,
                    ContactPhone = entity.DeliveryInfo.ContactPhone
                },
            Items = entity.Items.Select(item => new SelfBaseOrderResponse.ItemResponse
            {
                Id = item.Id,
                ArticleCode = ArticleCode.Parse(item.ArticleCode, null),
                ArticleTitle = item.ArticleTitle,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList(),
            StatusHistory = entity.StatusHistory.Select(status => new SelfBaseOrderResponse.StatusResponse
            {
                Status = status.Status,
                ChangedAt = status.ChangedAt,
                Note = status.Note
            }).ToList(),
            Payments = entity.Payments.Select(payment => new SelfBaseOrderResponse.PaymentSnapshotResponse
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