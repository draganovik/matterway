using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.ProductDetails;

public class RemoveProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Products/{productId:guid}/Details/{typeSlug}", Handle)
            .WithName("DeleteProductDetail").WithSummary("Delete a ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(RequestClaimsRole.Admin),
                nameof(RequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveProductDetailResponse>, NotFound>> Handle(
        Guid productId,
        string typeSlug,
        IProductDetailRepository productDetailRepository,
        CancellationToken cancellationToken)
    {
        var entity = await productDetailRepository.GetByKey(productId, typeSlug, cancellationToken);

        if (entity is null) return TypedResults.NotFound();

        var isDeleted = await productDetailRepository.Delete(productId, typeSlug, cancellationToken);

        return isDeleted ? TypedResults.Ok(MapToResponse(entity)) : TypedResults.NotFound();
    }

    public record RemoveProductDetailResponse
    {
        public required Guid ProductId { get; init; }
        public required string DetailType { get; init; }
        public string Message { get; init; } = "Product detail removed successfully.";
    }

    public static RemoveProductDetailResponse MapToResponse(ProductDetail entity)
    {
        return new RemoveProductDetailResponse
        {
            ProductId = entity.ProductId,
            DetailType = entity.Type?.Title ?? "Detail key: " + entity.TypeSlug
        };
    }
}