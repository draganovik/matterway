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
        app.MapDelete("ProductDetails/{id:guid}", Handle)
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
        Guid id,
        IProductDetailRepository productDetailRepository,
        CancellationToken cancellationToken)
    {
        var entity = await productDetailRepository.GetById(id, cancellationToken);

        if (entity is null)
        {
            return TypedResults.NotFound();
        }

        var isDeleted = await productDetailRepository.Delete(id, cancellationToken);

        return isDeleted ? TypedResults.Ok(MapToResponse(entity)) : TypedResults.NotFound();
    }

    public record RemoveProductDetailResponse
    {
        public required string DetailType { get; init; }
        public required Guid ProductId { get; init; }
        public string Message { get; init; } = "Product detail removed successfully.";
    }

    public static RemoveProductDetailResponse MapToResponse(ProductDetail entity)
    {
        return new RemoveProductDetailResponse
        {
            DetailType = entity.Title,
            ProductId = entity.ProductId,
        };
    }
}