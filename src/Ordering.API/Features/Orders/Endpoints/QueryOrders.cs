using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Features.Orders.Contracts;
using Ordering.API.Features.Orders.Data;
using Ordering.API.Features.Orders.Domain;
using Ordering.API.Features.Shared;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;
using Common.Infrastructure.ModelTemplates;

namespace Ordering.API.Features.Orders.Endpoints;

public class QueryOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders", Handler)
            .WithName("QueryOrders").WithSummary("Query Orders.")
            .WithTags(nameof(Order))
            .Produces<PaginationResponse<OrderBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Ok<PaginationResponse<OrderBaseResponse>>, NoContent, ForbidHttpResult, ValidationProblem>>
        Handler(
            [AsParameters]
            PagingQueryParams pagingQuery,
            HttpContext httpContext,
            IOrderRepository orderRepository,
            IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Forbid();

        int total;
        IEnumerable<Order>? entities;
        if (userRole == SystemUserRole.Customer)
        {
            total = await orderRepository.GetTotalEntities(systemUserId);
            entities = await orderRepository.QueryByCustomerId(systemUserId, pagingQuery.Page!.Value,
                pagingQuery.PageSize!.Value);
        }
        else
        {
            total = await orderRepository.GetTotalEntities();
            entities = await orderRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        }

        if (entities is not { } value || !value.Any()) return TypedResults.NoContent();

        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "Orders");
        var paginationResponse = new PaginationResponse<OrderBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<OrderBaseResponse>>(value).ToList(), baseUri);

        return TypedResults.Ok(paginationResponse);
    }
}