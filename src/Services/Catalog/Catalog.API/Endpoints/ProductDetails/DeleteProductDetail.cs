using System;
using Catalog.API.Entities;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Enums;
using Shared.Models;
using ProductDetail = Catalog.API.Entities.ProductDetail;

namespace Catalog.API.Endpoints.ProductDetails;

public class DeleteProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/ProductDetails/{id:guid}", Handler)
            .WithName("DeleteProductDetail").WithSummary("Delete a ProductDetail by id.")
            .WithTags(nameof(ProductDetail))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(
        Guid id,
        IProductDetailRepository productDetailRepository)
    {
        var isDeleted = await productDetailRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
