using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

namespace Matterway.Catalog.Api.Endpoints.Public.Articles;

public class PublicGetArticleByCode : IEndpoint
{
    private const string RouteName = nameof(PublicGetArticleByCode);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Public, "articles/{code:ArticleCode}", Handle)
            .WithName(RouteName).WithSummary("[public] Get an Article")
            .WithTags("Articles")
            .Produces<GetArticleByCodeResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<GetArticleByCodeResponse>, NotFound>> Handle(
        ArticleCode code,
        IArticleRepository articleRepository,
        CancellationToken cancellationToken)
    {
        var article = await articleRepository.GetBy(code, cancellationToken);

        if (article == null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(article));
    }

    public record GetArticleByCodeResponse
    {
        public required ArticleCode Code { get; init; }
        public string? Title { get; init; }
        public decimal? BasePrice { get; init; }
        public decimal? Price { get; init; }
        public ArticleDiscountProperty? Discount { get; set; }
        public string? Description { get; init; }
        public ICollection<ArticleDetailProperty>? Details { get; init; } = [];
        public ICollection<ArticleImageProperty>? Images { get; init; } = [];
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public bool IsAvailable { get; init; }
    }

    public record ArticleDiscountProperty
    {
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
    }

    public record ArticleDetailProperty
    {
        public string? DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
        public string? TextValue { get; init; }
        public decimal? NumericValue { get; init; }
    }

    public record ArticleImageProperty
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }

    public static GetArticleByCodeResponse MapToResponse(Article entity)
    {
        var basePrice = entity.BasePrice;
        var price = entity.GetFinalPrice();
        var discount = entity.GetLatestActiveDiscount();
        return new GetArticleByCodeResponse
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
            Details = MergeDetails(entity),
            Images = entity.ArticleImages?
                .OrderBy(pi => pi.OrderIndex)
                .Select(MapImageToResponse).ToList(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsAvailable = entity.IsAvailable
        };
    }

    public static ArticleDetailProperty MapDetailToResponse(ArticleDetailText entity)
    {
        return new ArticleDetailProperty
        {
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = entity.Value,
            NumericValue = null
        };
    }

    public static ArticleDetailProperty MapDetailToResponse(ArticleDetailNumeric entity)
    {
        return new ArticleDetailProperty
        {
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = null,
            NumericValue = entity.Value
        };
    }

    private static ICollection<ArticleDetailProperty> MergeDetails(Article entity)
    {
        var details = new List<ArticleDetailProperty>();
        if (entity.ArticleDetailTexts is not null)
            details.AddRange(entity.ArticleDetailTexts.Select(MapDetailToResponse));
        if (entity.ArticleDetailNumerics is not null)
            details.AddRange(entity.ArticleDetailNumerics.Select(MapDetailToResponse));

        return details
            .OrderBy(d => d.Title)
            .ToList();
    }

    public static ArticleImageProperty MapImageToResponse(ArticleImage entity)
    {
        return new ArticleImageProperty
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}