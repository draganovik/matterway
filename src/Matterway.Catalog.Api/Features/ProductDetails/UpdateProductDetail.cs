using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
        app.MapPatch("ProductDetails/{id:guid}", Handle)
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
            Guid id,
            UpdateProductDetailRequest request,
            IProductDetailRepository productDetailRepository,
            CancellationToken cancellationToken)
    {
        var entity = await productDetailRepository.GetById(id, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdates(entity, request);

        var updated = await productDetailRepository.UpdateAsync(entity, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateProductDetailRequest
    {
        [Required]
        public Guid ProductId { get; init; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DetailType Type { get; init; }

        [Required]
        public string? Title { get; init; }

        [Required]
        public string? Value { get; init; }

        public string? Unit { get; init; }
    }

    public record UpdateProductDetailResponse
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string? ProductTitle { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DetailType Type { get; init; }

        public string? Title { get; init; }
        public string? Value { get; init; }
        public string? Unit { get; init; }
    }

    public static void MapUpdates(ProductDetail entity, UpdateProductDetailRequest request)
    {
        entity.Type = request.Type;
        entity.Title = request.Title ?? entity.Title;
        entity.Value = request.Value ?? entity.Value;
        entity.Unit = request.Unit ?? entity.Unit;
    }

    public static UpdateProductDetailResponse MapToResponse(ProductDetail entity)
    {
        return new UpdateProductDetailResponse
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            ProductTitle = entity.Product?.Title,
            Type = entity.Type,
            Title = entity.Title,
            Value = entity.Value,
            Unit = entity.Unit
        };
    }
}