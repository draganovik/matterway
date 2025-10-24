using Asp.Versioning;
using Catalog.Api.Domain;
using Catalog.Api.Features.ProductImages.Contracts;
using Catalog.Api.Features.ProductImages.Mapping;
using Catalog.Api.Infrastructure.Abstractions;
using Common.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Catalog.Api.Features.ProductImages.Endpoints;

public class GetProductImageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("GetProductImageById").WithSummary("Get a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductImageBaseResponse>, NotFound>> Handler(
        Guid productId,
        int id,
        IProductImageRepository productImageRepository)
    {
        return await productImageRepository.GetById(productId, id)
            is { } value
            ? TypedResults.Ok(value.ToContract<ProductImageBaseResponse>())
            : TypedResults.NotFound();
    }
}