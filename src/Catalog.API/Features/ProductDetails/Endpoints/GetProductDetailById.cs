using Asp.Versioning;
using AutoMapper;
using Catalog.API.Features.ProductDetails.Contracts;
using Catalog.API.Features.ProductDetails.Data;
using Catalog.API.Features.ProductDetails.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Iterfaces;

namespace Catalog.API.Features.ProductDetails.Endpoints;

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