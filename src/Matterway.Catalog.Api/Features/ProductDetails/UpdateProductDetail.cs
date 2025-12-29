using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductDetails;

public class UpdateProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{productId:guid}/Details/{detailSlug}", Handle)
            .WithName("UpdateProductDetail").WithSummary("Update a ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<UpdateProductDetailResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateProductDetailResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handle(
            Guid productId,
            string detailSlug,
            UpdateProductDetailRequest request,
            IProductDetailRepository productDetailRepository,
            CancellationToken cancellationToken)
    {
        var entity = await productDetailRepository.GetBy(productId, detailSlug, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdates(entity, request);

        var updated = await productDetailRepository.Update(productId, detailSlug, entity, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateProductDetailRequest
    {
        [Required]
        public required string Value { get; init; }
    }

    public record UpdateProductDetailResponse
    {
        public string? ProductTitle { get; init; }
        public string? Detail { get; init; }
        public string? Value { get; init; }
    }

    public static void MapUpdates(ProductDetail entity, UpdateProductDetailRequest request)
    {
        entity.Value = request.Value;
    }

    public static UpdateProductDetailResponse MapToResponse(ProductDetail entity)
    {
        return new UpdateProductDetailResponse
        {
            ProductTitle = entity.Product?.Title,
            Detail = entity.Detail?.Title,
            Value = entity.Value
        };
    }
}