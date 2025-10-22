using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Customers.API.Features.CartItems.Contracts;
using Customers.API.Features.CartItems.Data;
using Customers.API.Features.CartItems.Domain;
using Customers.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;
using SharedProject.ModelTemplates;

namespace Customers.API.Features.CartItems.Endpoints;

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
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
        Results<Ok<PaginationResponse<CartItemBaseResponse>>, NoContent, ForbidHttpResult, ValidationProblem>> Handler(
        [AsParameters]
        PagingQueryParams pagingQuery,
        HttpContext httpContext,
        ICartItemRepository cartItemRepository,
        IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Forbid();

        int total;
        IEnumerable<CartItem> entities;

        if (userRole == SystemUserRole.Customer)
        {
            total = await cartItemRepository.GetTotalEntities(systemUserId);
            entities = await cartItemRepository.QueryByCustomerId(systemUserId, pagingQuery.Page!.Value,
                pagingQuery.PageSize!.Value);
        }
        else
        {
            total = await cartItemRepository.GetTotalEntities();
            entities = await cartItemRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        }

        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "Customers/CartItems");
        var paginationResponse = new PaginationResponse<CartItemBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<CartItemBaseResponse>>(entities).ToList(), baseUri);

        return entities.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}