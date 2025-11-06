using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductImages.Update;

public class UpdateProductImageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("UpdateProductImage").WithSummary("Update a ProductImage.")
            .WithTags(nameof(ProductImage))
            .Produces<UpdateProductImageResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateProductImageResponse>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid productId,
        int id,
        UpdateProductImageRequest request,
        IProductImageRepository productImageRepository)
    {
        var entity = await productImageRepository.GetById(productId, id);
        if (entity is null) return TypedResults.NotFound();

        entity.MapUpdates(request);

        var updated = await productImageRepository.UpdateAsync(entity);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(updated.ToResponse());
    }
}