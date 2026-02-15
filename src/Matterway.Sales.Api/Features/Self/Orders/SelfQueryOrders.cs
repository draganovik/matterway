using Asp.Versioning;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Self.Orders;

public class SelfQueryOrders : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "orders", Handler)
            .WithName("SelfQueryOrders").WithSummary("[self] Query own Orders.")
            .WithTags(nameof(Order))
            .Produces<PaginationResponse<SelfCreateOrder.OrderResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
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
            "SelfQueryOrders",
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

    public sealed record QueryOrdersParameters : PaginationRequestParameters;
}