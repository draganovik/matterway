using Asp.Versioning;
using Matterway.Catalog.Api.Application.Repositories;
using Matterway.Catalog.Api.Domain;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductDetails.Update;

public class UpdateProductDetailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("ProductDetails/{id:guid}", Handler)
            .WithName("UpdateProductDetail").WithSummary("Update a ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<UpdateProductDetailResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateProductDetailResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handler(
            Guid id,
            UpdateProductDetailRequest request,
            IProductDetailRepository productDetailRepository)
    {
        var entity = await productDetailRepository.GetById(id);
        if (entity is null) return TypedResults.NotFound();

        entity.MapUpdates(request);

        var updated = await productDetailRepository.UpdateAsync(entity);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(updated.ToResponse());
    }
}