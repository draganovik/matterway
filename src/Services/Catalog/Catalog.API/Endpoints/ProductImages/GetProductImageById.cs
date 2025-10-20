using Asp.Versioning;
using AutoMapper;
using Catalog.API.Models.ProductImageModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Iterfaces;
using ProductImage = Catalog.API.Entities.ProductImage;

namespace Catalog.API.Endpoints.ProductImages;

public class GetProductImageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("GetProductImageById").WithSummary("Get a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductImageBaseResponseModel>, NotFound>> Handler(
        Guid productId,
        int id,
        IProductImageRepository productImageRepository,
        IMapper mapper)
    {
        return await productImageRepository.GetById(productId, id)
            is ProductImage value
            ? TypedResults.Ok(mapper.Map<ProductImageBaseResponseModel>(value))
            : TypedResults.NotFound();
    }
}