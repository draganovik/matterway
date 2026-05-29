using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Self.Orders.Contracts;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Features.Self.Orders.Endpoints;

public class SelfQueryOrders : IEndpoint
{
    private const string RouteName = nameof(SelfQueryOrders);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "orders", Handler)
            .WithName(RouteName).WithSummary("[self] Query own Orders.")
            .WithTags(nameof(Order))
            .Produces<PaginationResponse<SelfBaseOrderResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async
        Task<Results<Ok<PaginationResponse<SelfBaseOrderResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] SelfQueryOrderParameters queryParameters,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            IOrderRepository orderRepository,
            CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.NoContent();

        var total = await orderRepository.Count(customerId, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await orderRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            customerId,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            RouteName,
            null);

        var results = entities.Select(ToResponse).ToList();

        var paginationResponse = PaginationResponse<SelfBaseOrderResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    private static SelfBaseOrderResponse ToResponse(Order entity)
    {
        var total = entity.Items.Sum(item => item.UnitPrice * item.Quantity);

        return new SelfBaseOrderResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Type = entity.Type,
            TotalAmount = Math.Round(total, 2, MidpointRounding.AwayFromZero),
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
