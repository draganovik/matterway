using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductDetails;

public class UpdateProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{productId:guid}/Details/{typeId:int}", Handle)
            .WithName("UpdateProductDetail").WithSummary("Update a ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<UpdateProductDetailResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateProductDetailResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handle(
            Guid productId,
            int typeId,
            UpdateProductDetailRequest request,
            IProductDetailRepository productDetailRepository,
            CancellationToken cancellationToken)
    {
        var entity = await productDetailRepository.GetByKey(productId, typeId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdates(entity, request);

        var updated = await productDetailRepository.UpdateAsync(productId, typeId, entity, cancellationToken);

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
        public string? Type { get; init; }
        public string? Value { get; init; }
        public string? Unit { get; init; }
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
            Type = entity.Type?.Title,
            Value = entity.Value,
            Unit = entity.Type?.Unit
        };
    }
}