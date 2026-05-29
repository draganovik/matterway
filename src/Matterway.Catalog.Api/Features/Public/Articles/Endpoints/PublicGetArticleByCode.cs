using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Public.Articles.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using DetailResponse = Matterway.Catalog.Api.Features.Public.Articles.Contracts.PublicGetArticleByCodeResponse.DetailResponse;
using DiscountResponse = Matterway.Catalog.Api.Features.Public.Articles.Contracts.PublicGetArticleByCodeResponse.DiscountResponse;
using ImageResponse = Matterway.Catalog.Api.Features.Public.Articles.Contracts.PublicGetArticleByCodeResponse.ImageResponse;

namespace Matterway.Catalog.Api.Features.Public.Articles.Endpoints;

public class PublicGetArticleByCode : IEndpoint
{
    private const string RouteName = nameof(PublicGetArticleByCode);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Public, "articles/{code:ArticleCode}", Handle)
            .WithName(RouteName).WithSummary("[public] Get an Article")
            .WithTags("Articles")
            .Produces<PublicGetArticleByCodeResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<PublicGetArticleByCodeResponse>, NotFound>> Handle(
        ArticleCode code,
        IArticleRepository articleRepository,
        CancellationToken cancellationToken)
    {
        var article = await articleRepository.GetBy(code, cancellationToken);

        if (article == null) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(article));
    }

    private static PublicGetArticleByCodeResponse ToResponse(Article entity)
    {
        var basePrice = entity.BasePrice;
        var price = entity.GetFinalPrice();
        var discount = entity.GetLatestActiveDiscount();
        return new PublicGetArticleByCodeResponse
        {
            Code = ArticleCode.Parse(entity.ArticleCode, null),
            Title = entity.Title,
            BasePrice = basePrice,
            Price = price,
            Discount = discount is null
                ? null
                : new DiscountResponse
                {
                    Percentage = discount.Percentage,
                    ValidFrom = discount.ValidFrom,
                    ValidTo = discount.ValidTo
                },
            Description = entity.Description,
            Details = MergeDetails(entity),
            Images = entity.ArticleImages?
                .OrderBy(pi => pi.OrderIndex)
                .Select(ToImageResponse).ToList(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsAvailable = entity.IsAvailable
        };
    }

    private static DetailResponse ToDetailResponse(ArticleDetailText entity)
    {
        return new DetailResponse
        {
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = entity.Value,
            NumericValue = null
        };
    }

    private static DetailResponse ToDetailResponse(ArticleDetailNumeric entity)
    {
        return new DetailResponse
        {
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = null,
            NumericValue = entity.Value
        };
    }

    private static ICollection<DetailResponse> MergeDetails(Article entity)
    {
        var details = new List<DetailResponse>();
        if (entity.ArticleDetailTexts is not null)
            details.AddRange(entity.ArticleDetailTexts.Select(ToDetailResponse));
        if (entity.ArticleDetailNumerics is not null)
            details.AddRange(entity.ArticleDetailNumerics.Select(ToDetailResponse));

        return details
            .OrderBy(d => d.Title)
            .ToList();
    }

    private static ImageResponse ToImageResponse(ArticleImage entity)
    {
        return new ImageResponse
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}
