using Asp.Versioning;
using AutoMapper;
using Catalog.Api.Features.ProductDetails.Contracts;
using Catalog.Api.Features.ProductDetails.Data;
using Catalog.Api.Features.ProductDetails.Domain;
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
            .Produces<ProductDetailBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductDetailBaseResponse>, NotFound>> Handler(
        Guid id,
        IProductDetailRepository productDetailRepository,
        IMapper mapper)
    {
        return await productDetailRepository.GetById(id)
            is ProductDetail value
            ? TypedResults.Ok(mapper.Map<ProductDetailBaseResponse>(value))
            : TypedResults.NotFound();
    }
}