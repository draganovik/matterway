using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.ProductImages;

public class RemoveProductImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Products/{productId:guid}/Images/{orderIndex:int}", Handle)
            .WithName("RemoveProductImage").WithSummary("Remove a ProductImage.")
            .WithTags(nameof(ProductImage))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveProductImageResponse>, NotFound>> Handle(
        Guid productId,
        int orderIndex,
        IProductImageRepository productImageRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        var entity = await productImageRepository.GetByOrderIndex(productId, orderIndex, cancellationToken);

        if (entity is null) return TypedResults.NotFound();

        var isDeleted = await productImageRepository.Delete(productId, orderIndex, cancellationToken);

        if (!isDeleted) return TypedResults.NotFound();

        await imageStorageService.DeleteAsync(entity.ProductId, entity.Id, cancellationToken);
        return TypedResults.Ok(MapToResponse(entity));
    }

    public record RemoveProductImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public required string ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
        public required Guid ProductId { get; init; }
        public string Message { get; init; } = "Product image removed successfully.";
    }

    public static RemoveProductImageResponse MapToResponse(ProductImage entity)
    {
        return new RemoveProductImageResponse
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ProductId = entity.ProductId,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}