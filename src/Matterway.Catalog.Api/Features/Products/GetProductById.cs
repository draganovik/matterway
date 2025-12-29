using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductEntity;
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

        if (product == null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(product));
    }

    public record GetProductByIdResponse
    {
        public Guid Id { get; init; }
        public string? Code { get; init; }
        public string? Title { get; init; }
        public decimal? BasePrice { get; init; }
        public decimal? Price { get; init; }
        public ProductDiscountProperty? Discount { get; set; }
        public string? Description { get; init; }
        public ICollection<ProductDetailProperty>? Details { get; init; } = [];
        public ICollection<ProductSpecificationProperty>? Specifications { get; init; } = [];
        public ICollection<ProductImageProperty>? Images { get; init; } = [];
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public bool IsAvailable { get; init; }
    }

    public record ProductDiscountProperty
    {
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
    }

    public record ProductDetailProperty
    {
        public string? DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Value { get; init; }
    }

    public record ProductSpecificationProperty
    {
        public string? SpecificationSlug { get; init; }
        public string? Title { get; init; }
        public decimal Value { get; init; }
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
        var basePrice = entity.GetBasePrice(ESupportedCurrency.RSD);
        var price = entity.GetFinalPrice(ESupportedCurrency.RSD);
        var discount = entity.GetLatestActiveDiscount(ESupportedCurrency.RSD);
        return new GetProductByIdResponse
        {
            Id = entity.Id,
            Code = entity.ProductCode,
            Title = entity.Title,
            BasePrice = basePrice,
            Price = price,
            Discount = discount is null
                ? null
                : new ProductDiscountProperty
                {
                    Percentage = discount.Percentage,
                    ValidFrom = discount.ValidFrom,
                    ValidTo = discount.ValidTo
                },
            Description = entity.Description,
            Details = entity.ProductDetails?
                .Select(MapDetailToResponse).ToList(),
            Specifications = entity.ProductSpecifications?
                .Select(MapSpecificationToResponse).ToList(),
            Images = entity.ProductImages?
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
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Value = entity.Value
        };
    }

    public static ProductSpecificationProperty MapSpecificationToResponse(ProductSpecification entity)
    {
        return new ProductSpecificationProperty
        {
            SpecificationSlug = entity.SpecificationSlug,
            Title = entity.Specification?.Title,
            Value = entity.Value,
            Unit = entity.Specification?.Unit
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