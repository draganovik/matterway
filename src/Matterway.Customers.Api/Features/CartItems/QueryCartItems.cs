using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;

namespace Matterway.Customers.Api.Features.CartItems;

public class QueryCartItems(ICartItemRepository cartItemRepository, LinkGenerator linkGenerator) :
    Endpoint<PaginationRequestParameters, PaginationResponse<QueryCartItems.CartItemResponse>>
{
    public override void Configure()
    {
        Get("/customers/cartitems");
        Version(1);
        Options(options =>
        {
            options.WithName("QueryCartItems")
                .WithSummary("Query CartItems.")
                .WithTags(nameof(CartItem))
                .Produces<PaginationResponse<CartItemResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status403Forbidden)
                .RequireAuthorization(policy => policy.RequireRole(
                    nameof(ERequestClaimsRole.Customer)));
        });
    }

    public override async Task HandleAsync(PaginationRequestParameters request, CancellationToken cancellationToken)
    {
        if (!UserContext.TryGet(User, out var userContext))
        {
            await Send.ForbiddenAsync(cancellationToken);
            return;
        }

        var total = await cartItemRepository.Count(userContext.SystemUserId);
        var entities = await cartItemRepository.QueryForSuid(userContext.SystemUserId, request.Page, request.PageSize);

        var baseUri = linkGenerator.GetUriByName(HttpContext, "QueryCartItems", null);
        var response = entities.Select(MapToResponse).ToList();

        var paginationResponse = PaginationResponse<CartItemResponse>.Create(
            response,
            total,
            request.Page,
            request.PageSize,
            baseUri);

        if (!entities.Any())
        {
            await Send.NoContentAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(paginationResponse, cancellationToken);
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