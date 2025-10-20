using System;
using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductDetailModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Models;
using ProductDetail = Catalog.API.Entities.ProductDetail;

namespace Catalog.API.Endpoints.ProductDetails;

public class GetProductDetailById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/ProductDetails/{id:guid}", Handler)
            .WithName("GetProductDetailById").WithSummary("Get a ProductDetail by id.")
            .WithTags(nameof(ProductDetail))
            .Produces<ProductDetailBaseResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<ProductDetailBaseResponseModel>, NotFound>> Handler(
        Guid id,
        IProductDetailRepository productDetailRepository,
        IMapper mapper)
    {
        return await productDetailRepository.GetById(id)
            is ProductDetail value
            ? TypedResults.Ok(mapper.Map<ProductDetailBaseResponseModel>(value))
            : TypedResults.NotFound();
    }
}
