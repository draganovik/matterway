using Asp.Versioning;
using Catalog.API.Features.ProductDetails.Data;
using Catalog.API.Features.ProductDetails.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Catalog.API.Features.ProductDetails.Endpoints;

public class DeleteProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("ProductDetails/{id:guid}", Handler)
            .WithName("DeleteProductDetail").WithSummary("Delete a ProductDetail by id.")
            .WithTags(nameof(ProductDetail))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(
        Guid id,
        IProductDetailRepository productDetailRepository)
    {
        var isDeleted = await productDetailRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}