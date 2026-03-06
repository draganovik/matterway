using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

namespace Matterway.Catalog.Api.Endpoints.Public.Articles;

public class PublicQueryArticles : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Public, "articles", Handle)
            .WithName("PublicQueryArticles").WithSummary("[public] Query Articles")
            .WithTags("Articles")
            .Produces<PaginationResponse<QueryArticleResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1))
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                var filterParam = operation.Parameters?
                    .FirstOrDefault(p => string.Equals(p.Name, nameof(QueryArticlesParameters.Filter),
                        StringComparison.OrdinalIgnoreCase));

                const string filterDescription =
                    "RSQL filter string. Use ';' for AND and ',' for OR. Details (text) support ==, !=, in, out; " +
                    "numeric details support eq, !=, ge, le, in, out. " +
                    "Fields: title, code, description, price, available, detail slugs.";

                filterParam?.Description = filterDescription;

                return Task.CompletedTask;
            });
    }

    private static async Task<Results<Ok<PaginationResponse<QueryArticleResponse>>, NoContent>>
        Handle([AsParameters] QueryArticlesParameters queryParameters,
            HttpContext httpContext, LinkGenerator linkGenerator, IArticleRepository articleRepository,
            CancellationToken cancellationToken)
    {
        var total = await articleRepository.Count(queryParameters.Filter, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await articleRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.Filter,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            "PublicQueryArticles",
            null);

        var results = entities.Select(MapToResponse).ToList();

        var paginationResponse = PaginationResponse<QueryArticleResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    public sealed record QueryArticlesParameters : PaginationRequestParameters
    {
        /// <summary>
        /// RSQL filter string. Use ';' for AND, and ',' for OR. Text fields (details) support ==/!=/in/out;
        /// numeric fields (price, numeric details) support eq/!=/ge/le/in/out. Fields:
        /// title, code, description, price, available, and detail slugs.
        /// NOTE: eq and == are equivalent and validate if a field contains the given value for strings.
        /// </summary>
        public string? Filter { get; init; }
    }

    public record QueryArticleResponse
    {
        public required ArticleCode Code { get; set; }
        public string? Title { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? Price { get; set; }
        public ArticleDiscountProperty? Discount { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailAlt { get; set; }
        public bool IsAvailable { get; set; }
    }

    public record ArticleDiscountProperty
    {
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
    }

    public static QueryArticleResponse MapToResponse(Article entity)
    {
        var basePrice = entity.BasePrice;
        var price = entity.GetFinalPrice();
        var discount = entity.GetLatestActiveDiscount();
        return new QueryArticleResponse
        {
            Code = ArticleCode.Parse(entity.ArticleCode, null),
            Title = entity.Title,
            BasePrice = basePrice,
            Price = price,
            Discount = discount is null
                ? null
                : new ArticleDiscountProperty
                {
                    Percentage = discount.Percentage,
                    ValidFrom = discount.ValidFrom,
                    ValidTo = discount.ValidTo
                },
            Description = entity.Description,
            ThumbnailUrl = entity.ArticleImages?
                .OrderBy(pi => pi.OrderIndex)
                .FirstOrDefault()
                ?.ImageUrl,
            ThumbnailAlt = entity.ArticleImages?
                .OrderBy(pi => pi.OrderIndex)
                .FirstOrDefault()
                ?.ImageAlt,
            IsAvailable = entity.IsAvailable
        };
    }
}