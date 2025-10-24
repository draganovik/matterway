using Asp.Versioning;
using Catalog.Api.Domain;
using Catalog.Api.Features.ProductImages.Contracts;
using Catalog.Api.Features.ProductImages.Data;
using Catalog.Api.Features.ProductImages.Mapping;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Features.ProductImages.Endpoints;

public class UpdateProductImageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("UpdateProductImageById").WithSummary("Update a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductImageBaseResponse>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid productId,
        int id,
        ProductImageBaseRequest request,
        IProductImageRepository productImageRepository)
    {
        var updatedProductImage = await productImageRepository.Update(productId, id, request);
        return updatedProductImage is not null
            ? TypedResults.Ok(updatedProductImage.ToContract<ProductImageBaseResponse>())
            : TypedResults.NotFound();
    }
}