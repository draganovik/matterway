using System;
using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Product = Catalog.API.Entities.Product;

namespace Catalog.API.Endpoints.Products;

public class GetProductById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Products/{id:guid}", Handler)
            .WithName("GetProductById").WithSummary("Get a Product by id.")
            .WithTags("Products")
            .Produces<ProductBaseResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<ProductBaseResponseModel>, NotFound>> Handler(
        Guid id,
        IProductRepository productRepository,
        IMapper mapper)
    {
        return await productRepository.GetById(id)
            is { } value
            ? TypedResults.Ok(mapper.Map<ProductBaseResponseModel>(value))
            : TypedResults.NotFound();
    }
}
