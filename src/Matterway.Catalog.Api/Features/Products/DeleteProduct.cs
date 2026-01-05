using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Products;

public class DeleteProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Products/{id:guid}", Handle)
            .WithName("DeleteProduct").WithSummary("Delete a Product.")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteProductResponse>, NotFound>> Handle(
        Guid id,
        IProductRepository productRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetBy(id, cancellationToken);

        if (product is null) return TypedResults.NotFound();

        var imageIds = product.ProductImages?
            .Select(image => new { image.ProductId, image.Id })
            .ToList();

        var isDeleted = await productRepository.Delete(id, cancellationToken);
        if (!isDeleted) return TypedResults.NotFound();

        if (imageIds is not null)
            foreach (var image in imageIds)
                await imageStorageService.DeleteAsync(image.ProductId, image.Id, cancellationToken);

        var response = new DeleteProductResponse
        {
            Id = id
        };
        return TypedResults.Ok(response);
    }

    public record DeleteProductResponse
    {
        public Guid Id { get; init; }
        public string Message { get; init; } = "Product deleted successfully.";
    }
}