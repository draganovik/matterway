using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;

namespace Matterway.Customers.Api.Features.CartItems;

public class QueryCartItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/CartItems", Handler)
            .WithName("QueryCartItems").WithSummary("Query CartItems.")
            .WithTags(nameof(CartItem))
            .Produces<PaginationResponse<CartItemResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Customer)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<CartItemResponse>>, NoContent, ForbidHttpResult>>
        Handler(
            [AsParameters]
            PaginationRequestParameters pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICartItemRepository cartItemRepository)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        var total = await cartItemRepository.Count(systemUserId);
        var entities = await cartItemRepository.QueryForSuid(systemUserId, pagingQuery.Page,
            pagingQuery.PageSize);

        var baseUri = linkGenerator.GetUriByName(httpContext, "QueryCartItems", null);
        var response = entities.Select(MapToResponse).ToList();

        var paginationResponse = PaginationResponse<CartItemResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        return entities.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }

    public record CartItemResponse
    {
        [Required]
        public Guid CustomerId { get; init; }

        [Required]
        public string? ProductName { get; init; }

        [Required]
        public Guid ProductId { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double UnitPrice { get; init; }
    }

    private static CartItemResponse MapToResponse(CartItem entity)
    {
        return new CartItemResponse
        {
            CustomerId = entity.CustomerId,
            ProductId = entity.ProductId,
            ProductName = entity.ProductName,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice
        };
    }
}