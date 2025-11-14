using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Enums;
using Matterway.Common.Pagination;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Matterway.Customers.Api.Extensions;
using Matterway.Customers.Api.Features.CartItems.Contracts;
using Matterway.Customers.Api.Features.CartItems.Data;
using Matterway.Customers.Api.Features.CartItems.Domain;

namespace Matterway.Customers.Api.Features.CartItems.Endpoints;

public class QueryCartItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/CartItems", Handler)
            .WithName("QueryCartItems").WithSummary("Query CartItems.")
            .WithTags(nameof(CartItem))
            .Produces<PaginationResponse<CartItemBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Customer)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
        Results<Ok<PaginationResponse<CartItemBaseResponse>>, NoContent, ForbidHttpResult, ValidationProblem>> Handler(
        [AsParameters]
        PagingQueryParams pagingQuery,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        ICartItemRepository cartItemRepository,
        IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        var total = await cartItemRepository.GetTotalEntities(systemUserId);
        var entities = await cartItemRepository.QueryByCustomerId(systemUserId, pagingQuery.Page!.Value,
            pagingQuery.PageSize.Value);

        var baseUri = linkGenerator.GetPathByName(httpContext, "QueryCartItems", null);
        var paginationResponse = new PaginationResponse<CartItemBaseResponse>(total, pagingQuery.Page.Value,
            pagingQuery.PageSize.Value,
            mapper.Map<IEnumerable<CartItemBaseResponse>>(entities).ToList(), new Uri(baseUri));

        return entities.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}