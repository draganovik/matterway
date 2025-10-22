using Asp.Versioning;
using Catalog.Api.Features.Products.Data;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Catalog.Api.Features.Products.Endpoints;

public class DeleteProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Products/{id:guid}", Handler)
            .WithName("DeleteProduct").WithSummary("Delete a Product by id.")
            .WithTags("Products")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(
        Guid id,
        IProductRepository productRepository)
    {
        var isDeleted = await productRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}