using Asp.Versioning;
using Matterway.Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Products.Delete;

public class DeleteProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Products/{id:guid}", Handler)
            .WithName("DeleteProduct").WithSummary("Delete a Product by id.")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteProductResponse>, NotFound>> Handler(
        Guid id,
        IProductRepository productRepository)
    {
        var isDeleted = await productRepository.Delete(id);
        var response = new DeleteProductResponse
        {
            Id = id
        };
        return isDeleted ? TypedResults.Ok(response) : TypedResults.NotFound();
    }
}