using Asp.Versioning;
using AutoMapper;
using Catalog.API.Features.ProductImages.Contracts;
using Catalog.API.Features.ProductImages.Data;
using Catalog.API.Features.ProductImages.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Common.Infrastructure.Interfaces;

namespace Catalog.API.Features.ProductImages.Endpoints;

public class GetProductImageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("GetProductImageById").WithSummary("Get a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductImageBaseResponse>, NotFound>> Handler(
        Guid productId,
        int id,
        IProductImageRepository productImageRepository,
        IMapper mapper)
    {
        return await productImageRepository.GetById(productId, id)
            is ProductImage value
            ? TypedResults.Ok(mapper.Map<ProductImageBaseResponse>(value))
            : TypedResults.NotFound();
    }
}