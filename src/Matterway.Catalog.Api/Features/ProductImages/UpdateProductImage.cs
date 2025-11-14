using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductImages;

public class UpdateProductImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{productId:guid}/Images/{orderIndex:int}", Handle)
            .WithName("UpdateProductImage").WithSummary("Update a ProductImage.")
            .WithTags(nameof(ProductImage))
            .Produces<UpdateProductImageResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(RequestClaimsRole.Admin),
                nameof(RequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateProductImageResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        Guid productId,
        int orderIndex,
        UpdateProductImageRequest request,
        IProductImageRepository productImageRepository,
        CancellationToken cancellationToken)
    {
        var entity = await productImageRepository.GetByOrderIndex(productId, orderIndex, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdates(entity, request);

        var targetOrderIndex = request.OrderIndex ?? entity.OrderIndex;

        if (targetOrderIndex < 0) targetOrderIndex = 0;

        var updated = await productImageRepository.UpdateAsync(entity, targetOrderIndex, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateProductImageRequest
    {
        public string? ImageAlt { get; init; }

        [Range(0, int.MaxValue)]
        public int? OrderIndex { get; init; }
    }

    public record UpdateProductImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public Guid ProductId { get; init; }
        public string? ProductName { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }

    public static void MapUpdates(ProductImage entity, UpdateProductImageRequest request)
    {
        entity.ImageAlt = request.ImageAlt ?? entity.ImageAlt;
    }

    public static UpdateProductImageResponse MapToResponse(ProductImage entity)
    {
        return new UpdateProductImageResponse
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ProductId = entity.ProductId,
            ProductName = entity.Product?.Title,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}