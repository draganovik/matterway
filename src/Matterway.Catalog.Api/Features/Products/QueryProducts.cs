using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

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
                    "RSQL filter string. Use ';' for AND and ',' for OR. Operators: eq, !=, ge, le, in, out. " +
                    "Fields: title, code, description, price, available, and configured product detail slugs.";

                filterParam?.Description = filterDescription;

                return Task.CompletedTask;
            });
    }

    private static async Task<Results<Ok<PaginationResponse<QueryProductResponse>>, NoContent>>
        Handle([AsParameters] QueryProductsParameters queryParameters,
            HttpContext httpContext, LinkGenerator linkGenerator, IProductRepository productRepository,
            CancellationToken cancellationToken)
    {
        var total = await productRepository.GetTotalEntities(queryParameters.Filter, cancellationToken);
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
        /// RSQL filter string. Use ';' for AND, and ',' for OR.
        /// Operators: eq, !=, ge, le, in, out.
        /// Fields: title, code, description, price, available, and configured product detail slugs.
        /// NOTE: eq and == are equivalent and validate if a field contains the given value.
        /// </summary>
        public string? Filter { get; init; }
    }

    public record QueryProductResponse
    {
        public Guid Id { get; set; }
        public string? ProductCode { get; set; }
        public string? Title { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
        public ProductPropertyImage? ThumbnailImage { get; set; }
        public bool IsAvailable { get; set; }
    }

    public record ProductPropertyImage
    {
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
    }

    public static QueryProductResponse MapToResponse(Product entity)
    {
        return new QueryProductResponse
        {
            Id = entity.Id,
            ProductCode = entity.ProductCode,
            Title = entity.Title,
            Price = entity.Price,
            Description = entity.Description,
            ThumbnailImage = entity.ProductImages?
                .OrderBy(pi => pi.OrderIndex)
                .Select(MapImageToResponse)
                .FirstOrDefault(),
            IsAvailable = entity.IsAvailable
        };
    }

    public static ProductPropertyImage MapImageToResponse(ProductImage entity)
    {
        return new ProductPropertyImage
        {
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}