using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.ProductDetails.Remove;

public class DeleteProductDetailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("ProductDetails/{id:guid}", Handler)
            .WithName("DeleteProductDetail").WithSummary("Delete a ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveProductDetailResponse>, NotFound>> Handler(
        Guid id,
        IProductDetailRepository productDetailRepository)
    {
        var entity = await productDetailRepository.GetById(id);

        if (entity is null)
        {
            return TypedResults.NotFound();
        }

        var isDeleted = await productDetailRepository.Delete(id);

        return isDeleted ? TypedResults.Ok(entity.ToResponse()) : TypedResults.NotFound();
    }
}