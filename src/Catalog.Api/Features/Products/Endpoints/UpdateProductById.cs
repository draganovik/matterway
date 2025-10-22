using Asp.Versioning;
using Catalog.Api.Features.Products.Contracts;
using Catalog.Api.Features.Products.Data;
using Catalog.Api.Features.Products.Mapping;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Features.Products.Endpoints;

public class UpdateProductById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{id:guid}", Handler)
            .WithName("UpdateProductById").WithSummary("Update a Product by id.")
            .WithTags("Products")
            .Produces<ProductBaseResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductBaseResponse>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid id,
        ProductBaseRequest request,
        IProductRepository productRepository)
    {
        var updatedProduct = await productRepository.Update(id, request);
        return updatedProduct is not null
            ? TypedResults.Ok(updatedProduct.ToContract<ProductBaseResponse>())
            : TypedResults.NotFound();
    }
}