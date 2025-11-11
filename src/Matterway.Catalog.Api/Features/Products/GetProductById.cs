using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Products;

public class GetProductById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Products/{id:guid}", Handle)
            .WithName("GetProductById").WithSummary("Get a Product.")
            .WithTags("Products")
            .Produces<GetProductByIdResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<GetProductByIdResponse>, NotFound>> Handle(
        Guid id,
        IProductRepository productRepository,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetById(id, cancellationToken);

        if (product == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(MapToResponse(product));
    }

    public record GetProductByIdResponse
    {
        public Guid Id { get; init; }
        public string? ProductCode { get; init; }
        public string? Title { get; init; }
        public double? Price { get; init; }
        public string? Description { get; init; }
        public ICollection<ProductDetailProperty>? ProductDetails { get; init; } = [];
        public ICollection<ProductImageProperty>? ProductImages { get; init; } = [];
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public bool IsAvailable { get; init; }
    }

    public record ProductDetailProperty
    {
        public int TypeId { get; init; }
        public string? Title { get; init; }
        public string? Value { get; init; }
        public string? Unit { get; init; }
    }

    public record ProductImageProperty
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }

    public static GetProductByIdResponse MapToResponse(Product entity)
    {
        return new GetProductByIdResponse
        {
            Id = entity.Id,
            ProductCode = entity.ProductCode,
            Title = entity.Title,
            Price = entity.Price,
            Description = entity.Description,
            ProductDetails = entity.ProductDetails?
                .Select(MapDetailToResponse).ToList(),
            ProductImages = entity.ProductImages?
                .OrderBy(pi => pi.OrderIndex)
                .Select(MapImageToResponse).ToList(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsAvailable = entity.IsAvailable
        };
    }

    public static ProductDetailProperty MapDetailToResponse(ProductDetail entity)
    {
        return new ProductDetailProperty
        {
            TypeId = entity.TypeId,
            Title = entity.Type?.Title,
            Value = entity.Value,
            Unit = entity.Type?.Unit
        };
    }

    public static ProductImageProperty MapImageToResponse(ProductImage entity)
    {
        return new ProductImageProperty
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}