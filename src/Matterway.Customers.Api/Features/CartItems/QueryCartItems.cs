using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.CartItems;

public class QueryCartItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/CartItems", Handler)
            .WithName("QueryCartItems").WithSummary("Query CartItems.")
            .WithTags(nameof(CustomerArticle))
            .Produces<PaginationResponse<CartItemResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Customer)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<CartItemResponse>>, NoContent, ForbidHttpResult>>
        Handler(
            [AsParameters]
            PaginationRequestParameters pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerArticleRepository cartItemRepository)
    {
        if (!RequestIdentity.TryGet(httpContext.User, out var userContext)) return TypedResults.Forbid();

        var total = await cartItemRepository.CountCart(userContext.SystemUserId);
        var entities = await cartItemRepository.QueryCart(userContext.SystemUserId, pagingQuery.Page,
            pagingQuery.PageSize);

        var baseUri = linkGenerator.GetUriByName(httpContext, "QueryCartItems", null);
        var response = entities.Select(MapToResponse).ToList();

        var paginationResponse = PaginationResponse<CartItemResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        if (entities.Count == 0) return TypedResults.NoContent();

        return TypedResults.Ok(paginationResponse);
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
        [Range(0.01, int.MaxValue)]
        public decimal? UnitPrice { get; init; }
    }

    private static CartItemResponse MapToResponse(CustomerArticle entity)
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