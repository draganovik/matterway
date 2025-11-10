using Asp.Versioning;
using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Products;

public class DeleteProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Products/{id:guid}", Handle)
            .WithName("DeleteProduct").WithSummary("Delete a Product.")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteProductResponse>, NotFound>> Handle(
        Guid id,
        IProductRepository productRepository,
        CancellationToken cancellationToken)
    {
        var isDeleted = await productRepository.Delete(id, cancellationToken);
        var response = new DeleteProductResponse
        {
            Id = id
        };
        return isDeleted ? TypedResults.Ok(response) : TypedResults.NotFound();
    }

    public record DeleteProductResponse
    {
        public Guid Id { get; init; }
        public string Message { get; init; } = "Product deleted successfully.";
    }
}