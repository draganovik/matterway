using Asp.Versioning;
using AutoMapper;
using Catalog.Api.Features.Products.Contracts;
using Catalog.Api.Features.Products.Data;
using Common.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Catalog.Api.Features.Products.Endpoints;

public class GetProductById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Products/{id:guid}", Handler)
            .WithName("GetProductById").WithSummary("Get a Product by id.")
            .WithTags("Products")
            .Produces<ProductBaseResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductBaseResponse>, NotFound>> Handler(
        Guid id,
        IProductRepository productRepository,
        IMapper mapper)
    {
        return await productRepository.GetById(id)
            is { } value
            ? TypedResults.Ok(mapper.Map<ProductBaseResponse>(value))
            : TypedResults.NotFound();
    }
}