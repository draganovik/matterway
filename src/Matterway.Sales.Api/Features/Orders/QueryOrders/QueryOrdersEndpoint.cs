using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Orders.CreateOrder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Orders.QueryOrders;

public class QueryOrdersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders", Handler)
            .WithName("QueryOrders").WithSummary("Query Orders.")
            .WithTags(nameof(Order))
            .Produces<PaginationResponse<OrderResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<OrderResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] QueryOrdersParameters queryParameters,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            QueryOrdersService queryOrdersService,
            CancellationToken cancellationToken)
    {
        var command = new QueryOrdersCommand(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.CustomerId);

        var total = await queryOrdersService.CountAsync(command, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await queryOrdersService.QueryAsync(command, cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            "QueryOrders");

        var results = entities.Select(CreateOrderMapper.MapToResponse).ToList();

        var paginationResponse = PaginationResponse<OrderResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }
}