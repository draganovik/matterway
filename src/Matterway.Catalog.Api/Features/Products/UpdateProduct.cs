using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Products;

public class UpdateProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{id:guid}", Handle)
            .WithName("UpdateProduct").WithSummary("Update a Product.")
            .WithTags("Products")
            .Produces<UpdateProductResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateProductResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        Guid id,
        UpdateProductRequest request,
        IProductRepository productRepository,
        CancellationToken cancellationToken)
    {
        var entity = await productRepository.GetById(id, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdate(entity, request);

        var updated = await productRepository.UpdateAsync(entity, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateProductRequest
    {
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Product code must be between 5 and 10 characters.")]
        [RegularExpression(@"^[A-Z0-9]{5,10}$",
            ErrorMessage = "Product code must contain only uppercase letters and numbers.")]
        public string? ProductCode { get; init; }

        [MinLength(1, ErrorMessage = "Title cannot be empty if provided.")]
        public string? Title { get; init; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal? Price { get; init; }

        [MinLength(1, ErrorMessage = "Description cannot be empty if provided.")]
        public string? Description { get; init; }

        public bool IsAvailable { get; init; }
    }

    public record UpdateProductResponse
    {
        public Guid Id { get; init; }
        public string? ProductCode { get; init; }
        public string? Title { get; init; }
        public decimal? Price { get; init; }
        public string? Description { get; init; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public bool IsAvailable { get; init; }
    }

    public static void MapUpdate(Product entity, UpdateProductRequest request)
    {
        entity.UpdateDetails(request.ProductCode, request.Title, request.Description);
        entity.SetAvailability(request.IsAvailable);

        if (request.Price is not null)
            entity.SetPrice(ESupportedCurrency.RSD, request.Price.Value);
    }

    public static UpdateProductResponse MapToResponse(Product entity)
    {
        return new UpdateProductResponse
        {
            Id = entity.Id,
            ProductCode = entity.ProductCode,
            Title = entity.Title,
            Price = entity.Prices?
                .FirstOrDefault(p => p.Currency == ESupportedCurrency.RSD)?
                .Amount,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsAvailable = entity.IsAvailable
        };
    }
}