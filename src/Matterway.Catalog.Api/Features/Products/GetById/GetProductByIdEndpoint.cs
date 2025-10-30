using Asp.Versioning;
using Matterway.Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Products.GetById;

public class GetProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Products/{id:guid}", Handler)
            .WithName("GetProductById").WithSummary("Get a Product by id.")
            .WithTags("Products")
            .Produces<GetProductByIdResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<GetProductByIdResponse>, NotFound>> Handler(
        Guid id,
        IProductRepository productRepository)
    {
        var product = await productRepository.GetById(id);

        if (product == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(product.ToResponse());
    }
}