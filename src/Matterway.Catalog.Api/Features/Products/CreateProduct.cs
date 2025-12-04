using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Products;

public class CreateProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Products", Handle)
            .WithName("CreateProduct").WithSummary("Create a new Product.")
            .WithTags("Products")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<CreateProductResponse>, BadRequest<ProblemDetails>>> Handle(
        CreateProductRequest request,
        IProductRepository productRepository,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        CancellationToken cancellationToken)
    {
        var product = MapToEntity(request);

        var created = await productRepository.Create(product, cancellationToken);

        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Product could not be created.",
                Status = StatusCodes.Status400BadRequest
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "GetProductById",
            new
            {
                id = created.Id
            });

        return TypedResults.Created(location, MapToResponse(created));
    }

    public record CreateProductRequest
    {
        [Required]
        [RegularExpression(@"^[A-Z0-9]{5,10}$",
            ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
        public required string ProductCode { get; init; }

        [Required]
        public required string Title { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public required decimal Price { get; init; }

        [Required]
        public required string Description { get; init; }

        public bool IsAvailable { get; init; }
    }

    public record CreateProductResponse
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

    public static Product MapToEntity(CreateProductRequest request)
    {
        var product = new Product
        {
            ProductCode = request.ProductCode,
            Title = request.Title,
            Description = request.Description
        };
        product.SetPrice(ESupportedCurrency.RSD, request.Price);
        product.SetAvailability(request.IsAvailable);
        return product;
    }

    public static CreateProductResponse MapToResponse(Product entity)
    {
        return new CreateProductResponse
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