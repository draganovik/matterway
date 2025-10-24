using Asp.Versioning;
using Catalog.Api.Domain;
using Catalog.Api.Features.ProductDetails.Contracts;
using Catalog.Api.Features.ProductDetails.Data;
using Catalog.Api.Features.ProductDetails.Mapping;
using Common.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Catalog.Api.Features.ProductDetails.Endpoints;

public class GetProductDetailById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ProductDetails/{id:guid}", Handler)
            .WithName("GetProductDetailById").WithSummary("Get a ProductDetail by id.")
            .WithTags(nameof(ProductDetail))
            .Produces<ProductDetailBaseResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductDetailBaseResponse>, NotFound>> Handler(
        Guid id,
        IProductDetailRepository productDetailRepository)
    {
        return await productDetailRepository.GetById(id)
            is { } value
            ? TypedResults.Ok(value.ToContract<ProductDetailBaseResponse>())
            : TypedResults.NotFound();
    }
}