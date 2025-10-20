using Asp.Versioning;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Enums;
using Shared.Iterfaces;
using ProductImage = Catalog.API.Entities.ProductImage;

namespace Catalog.API.Endpoints.ProductImages;

public class DeleteProductImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("DeleteProductImage").WithSummary("Delete a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(
        Guid productId,
        int id,
        IProductImageRepository productImageRepository)
    {
        var isDeleted = await productImageRepository.Delete(productId, id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}