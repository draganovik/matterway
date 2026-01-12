using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Providers.Persistence.ProductEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Products;

public class QueryProducts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Products", Handle)
            .WithName("QueryProducts").WithSummary("Query Products.")
            .WithTags("Products")
            .Produces<PaginationResponse<QueryProductResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0))
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                var filterParam = operation.Parameters?
                    .FirstOrDefault(p => string.Equals(p.Name, nameof(QueryProductsParameters.Filter),
                        StringComparison.OrdinalIgnoreCase));

                const string filterDescription =
                    "RSQL filter string. Use ';' for AND and ',' for OR. Details (text) support ==, !=, in, out; " +
                    "specifications (numeric) support eq, !=, ge, le, in, out. " +
                    "Fields: title, code, description, price, available, detail slugs, specification slugs.";

                filterParam?.Description = filterDescription;

                return Task.CompletedTask;
            });
    }

    private static async Task<Results<Ok<PaginationResponse<QueryProductResponse>>, NoContent>>
        Handle([AsParameters] QueryProductsParameters queryParameters,
            HttpContext httpContext, LinkGenerator linkGenerator, IProductRepository productRepository,
            CancellationToken cancellationToken)
    {
        var total = await productRepository.Count(queryParameters.Filter, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await productRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.Filter,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            "QueryProducts",
            null);

        var results = entities.Select(MapToResponse).ToList();

        var paginationResponse = PaginationResponse<QueryProductResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    public sealed record QueryProductsParameters : PaginationRequestParameters
    {
        /// <summary>
        /// RSQL filter string. Use ';' for AND, and ',' for OR. Text fields (details) support ==/!=/in/out;
        /// numeric fields (price, specifications) support eq/!=/ge/le/in/out. Fields:
        /// title, code, description, price, available, detail slugs, and specification slugs.
        /// NOTE: eq and == are equivalent and validate if a field contains the given value for strings.
        /// </summary>
        public string? Filter { get; init; }
    }

    public record QueryProductResponse
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Title { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? Price { get; set; }
        public ProductDiscountProperty? Discount { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailAlt { get; set; }
        public bool IsAvailable { get; set; }
    }

    public record ProductDiscountProperty
    {
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
    }

    public static QueryProductResponse MapToResponse(Product entity)
    {
        var basePrice = entity.GetBasePrice(ESupportedCurrency.RSD);
        var price = entity.GetFinalPrice(ESupportedCurrency.RSD);
        var discount = entity.GetLatestActiveDiscount(ESupportedCurrency.RSD);
        return new QueryProductResponse
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
            ThumbnailUrl = entity.ProductImages?
                .OrderBy(pi => pi.OrderIndex)
                .FirstOrDefault()
                ?.ImageUrl,
            ThumbnailAlt = entity.ProductImages?
                .OrderBy(pi => pi.OrderIndex)
                .FirstOrDefault()
                ?.ImageAlt,
            IsAvailable = entity.IsAvailable
        };
    }
}