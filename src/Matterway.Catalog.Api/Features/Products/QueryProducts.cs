using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Matterway.Common.Abstractions;
using Matterway.Common.Pagination;
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
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<QueryProductResponse>>, NoContent>>
        Handle([AsParameters] PagingQueryParams pagingQuery, [AsParameters] QueryProductFilter queryProductFilter,
            HttpContext httpContext, IProductRepository productRepository, CancellationToken cancellationToken)
    {
        var total = await productRepository.GetTotalEntities(queryProductFilter, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await productRepository.Query(
            pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            queryProductFilter,
            cancellationToken);

        var location = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}/Products");

        var results = entities.Select(MapToResponse).ToList();

        var paginationResponse = new PaginationResponse<QueryProductResponse>(
            total,
            pagingQuery.Page.Value,
            pagingQuery.PageSize.Value,
            results,
            location);

        return TypedResults.Ok(paginationResponse);
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

    public record QueryProductFilter
    {
        public string? TitleLike { get; set; }
        public double? PriceMin { get; set; }
        public double? PriceMax { get; set; }
        public string[]? ProductDetailsLike { get; set; }
        public bool? IsAvailable { get; set; }

        public IQueryable<Product> GenerateQuery(IQueryable<Product> query)
        {
            if (TitleLike != null)
                query = query.Where(p => p.Title.ToLower().Contains(TitleLike.ToLower()));
            if (PriceMin.HasValue) query = query.Where(p => p.Price >= PriceMin);
            if (PriceMax.HasValue) query = query.Where(p => p.Price <= PriceMax);
            if (ProductDetailsLike?.Length > 0)
                query = ProductDetailsLike.Aggregate(query,
                    (current, productDetailLike) => current.Where(p =>
                        p.ProductDetails != null && p.ProductDetails.Any(pd =>
                            pd.Value.ToLower().Contains(productDetailLike.ToLower()))));

            if (IsAvailable.HasValue) query = query.Where(p => p.IsAvailable == IsAvailable);

            return query;
        }
    }
}