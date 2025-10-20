using System;
using Catalog.API.Entities;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Enums;
using Shared.Models;
using ProductImage = Catalog.API.Entities.ProductImage;

namespace Catalog.API.Endpoints.ProductImages;

public class DeleteProductImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("DeleteProductImage").WithSummary("Delete a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)));
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
