using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductSpecificationEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.ProductSpecifications;

public class RemoveProductSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Products/{productId:guid}/Specifications/{specificationSlug}", Handle)
            .WithName("DeleteProductSpecification").WithSummary("Delete a ProductSpecification.")
            .WithTags(nameof(ProductSpecification))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveProductSpecificationResponse>, NotFound>> Handle(
        Guid productId,
        string specificationSlug,
        IProductSpecificationRepository repository,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetBy(productId, specificationSlug, cancellationToken);

        if (entity is null) return TypedResults.NotFound();

        var isDeleted = await repository.Delete(productId, specificationSlug, cancellationToken);

        return isDeleted ? TypedResults.Ok(MapToResponse(entity)) : TypedResults.NotFound();
    }

    public record RemoveProductSpecificationResponse
    {
        public required Guid ProductId { get; init; }
        public required string Specification { get; init; }
        public string Message { get; init; } = "Product specification removed successfully.";
    }

    public static RemoveProductSpecificationResponse MapToResponse(ProductSpecification entity)
    {
        return new RemoveProductSpecificationResponse
        {
            ProductId = entity.ProductId,
            Specification = entity.Specification?.Title ?? "Specification key: " + entity.SpecificationSlug
        };
    }
}