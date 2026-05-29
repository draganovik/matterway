using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Admin.Orders.Contracts;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Features.Admin.Orders.Endpoints;

public class AdminQueryOrders : IEndpoint
{
    private const string RouteName = nameof(AdminQueryOrders);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "orders", Handler)
            .WithName(RouteName).WithSummary("[admin] Query Orders")
            .WithTags(nameof(Order))
            .Produces<PaginationResponse<AdminBaseOrderResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async
        Task<Results<Ok<PaginationResponse<AdminBaseOrderResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] AdminQueryOrderParameters queryParameters,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            IOrderRepository orderRepository,
            CancellationToken cancellationToken)
    {
        var total = await orderRepository.Count(queryParameters.CustomerId, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await orderRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.CustomerId,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            RouteName,
            null);

        var results = entities.Select(ToResponse).ToList();

        var paginationResponse = PaginationResponse<AdminBaseOrderResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    private static AdminBaseOrderResponse ToResponse(Order entity)
    {
        var total = entity.Items.Sum(item => item.UnitPrice * item.Quantity);

        return new AdminBaseOrderResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Type = entity.Type,
            TotalAmount = Math.Round(total, 2, MidpointRounding.AwayFromZero),
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
