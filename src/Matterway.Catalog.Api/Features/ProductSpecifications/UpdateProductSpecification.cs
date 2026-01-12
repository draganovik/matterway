using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Providers.Persistence.ProductSpecificationEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductSpecifications;

public class UpdateProductSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{productId:guid}/Specifications/{specificationSlug}", Handle)
            .WithName("UpdateProductSpecification").WithSummary("Update a ProductSpecification.")
            .WithTags(nameof(ProductSpecification))
            .Produces<UpdateProductSpecificationResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateProductSpecificationResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handle(
            Guid productId,
            string specificationSlug,
            UpdateProductSpecificationRequest request,
            IProductSpecificationRepository repository,
            CancellationToken cancellationToken)
    {
        var entity = await repository.GetBy(productId, specificationSlug, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdates(entity, request);

        var updated = await repository.Update(productId, specificationSlug, entity, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateProductSpecificationRequest
    {
        [Required]
        public required decimal Value { get; init; }
    }

    public record UpdateProductSpecificationResponse
    {
        public string? ProductTitle { get; init; }
        public string? Specification { get; init; }
        public decimal Value { get; init; }
        public string? Unit { get; init; }
    }

    public static void MapUpdates(ProductSpecification entity, UpdateProductSpecificationRequest request)
    {
        entity.Value = request.Value;
    }

    public static UpdateProductSpecificationResponse MapToResponse(ProductSpecification entity)
    {
        return new UpdateProductSpecificationResponse
        {
            ProductTitle = entity.Product?.Title,
            Specification = entity.Specification?.Title,
            Value = entity.Value,
            Unit = entity.Specification?.Unit
        };
    }
}