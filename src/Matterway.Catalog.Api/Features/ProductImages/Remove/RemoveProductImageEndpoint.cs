using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.ProductImages.Remove;

public class RemoveProductImageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("RemoveProductImage").WithSummary("Remove a ProductImage.")
            .WithTags(nameof(ProductImage))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveProductImageResponse>, NotFound>> Handler(
        Guid productId,
        int id,
        IProductImageRepository productImageRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        var entity = await productImageRepository.GetById(productId, id);

        if (entity is null)
        {
            return TypedResults.NotFound();
        }

        var isDeleted = await productImageRepository.Delete(productId, id);

        if (!isDeleted)
        {
            return TypedResults.NotFound();
        }

        await imageStorageService.DeleteAsync(entity.ImageRef, cancellationToken);
        return TypedResults.Ok(entity.ToResponse());
    }
}
