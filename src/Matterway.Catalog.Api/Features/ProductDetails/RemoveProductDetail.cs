using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.ProductDetails;

public class RemoveProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Products/{productId:guid}/Details/{typeId:int}", Handle)
            .WithName("DeleteProductDetail").WithSummary("Delete a ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveProductDetailResponse>, NotFound>> Handle(
        Guid productId,
        int typeId,
        IProductDetailRepository productDetailRepository,
        CancellationToken cancellationToken)
    {
        var entity = await productDetailRepository.GetByKey(productId, typeId, cancellationToken);

        if (entity is null)
        {
            return TypedResults.NotFound();
        }

        var isDeleted = await productDetailRepository.Delete(productId, typeId, cancellationToken);

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
            DetailType = entity.Type?.Title ?? "Detail key: " + entity.TypeId,
        };
    }
}