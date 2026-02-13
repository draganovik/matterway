using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Self.Orders;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Admin.Orders;

public class AdminQueryOrders : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "orders", Handler)
            .WithName("AdminQueryOrders").WithSummary("[admin] Query Orders")
            .WithTags(nameof(Order))
            .Produces<PaginationResponse<SelfCreateOrder.OrderResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsObserver(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<PaginationResponse<SelfCreateOrder.OrderResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] QueryOrdersParameters queryParameters,
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
            "AdminQueryOrders",
            null);

        var results = entities.Select(SelfCreateOrder.MapToResponse).ToList();

        var paginationResponse = PaginationResponse<SelfCreateOrder.OrderResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    public sealed record QueryOrdersParameters : PaginationRequestParameters
    {
        public Guid? CustomerId { get; init; }
    }
}