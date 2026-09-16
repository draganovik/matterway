using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Public.Articles.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using DiscountResponse =
    Matterway.Catalog.Api.Features.Public.Articles.Contracts.PublicQueryArticleResponse.DiscountResponse;

namespace Matterway.Catalog.Api.Features.Public.Articles.Endpoints;

public class PublicQueryArticles : IEndpoint
{
    private const string RouteName = nameof(PublicQueryArticles);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Public, "articles", Handle)
            .WithName(RouteName).WithSummary("[public] Query Articles")
            .WithTags("Articles")
            .Produces<PaginationResponse<PublicQueryArticleResponse>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .MapToApiVersion(new ApiVersion(1))
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                var filterParam = operation.Parameters?
                    .FirstOrDefault(p => string.Equals(p.Name, nameof(PublicQueryArticleParameters.Filter),
                        StringComparison.OrdinalIgnoreCase));

                const string filterDescription =
                    "RSQL filter string. Use ';' for AND and ',' for OR. Details (text) support ==, !=, in, out; " +
                    "numeric details support eq, !=, ge, le, in, out. " +
                    "Fields: title, code, description, price, available, detail slugs.";

                filterParam?.Description = filterDescription;

                return Task.CompletedTask;
            });
    }

    private static async Task<Ok<PaginationResponse<PublicQueryArticleResponse>>>
        Handle([AsParameters] PublicQueryArticleParameters queryParameters,
            HttpContext httpContext, LinkGenerator linkGenerator, IArticleRepository articleRepository,
            CancellationToken cancellationToken)
    {
        var total = await articleRepository.Count(queryParameters.Filter, cancellationToken);
        var entities = await articleRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.Filter,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            RouteName);

        if (!string.IsNullOrWhiteSpace(location) && !string.IsNullOrWhiteSpace(queryParameters.Filter))
            location = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(location,
                nameof(PublicQueryArticleParameters.Filter),
                queryParameters.Filter);

        var results = entities.Select(ToResponse).ToList();

        var paginationResponse = PaginationResponse<PublicQueryArticleResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    private static PublicQueryArticleResponse ToResponse(Article entity)
    {
        var basePrice = entity.BasePrice;
        var price = entity.GetFinalPrice();
        var discount = entity.GetLatestActiveDiscount();
        var thumbnail = entity.ArticleImages?
            .OrderBy(pi => pi.OrderIndex)
            .FirstOrDefault();
        return new PublicQueryArticleResponse
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
            ThumbnailUrl = thumbnail?.ImageUrl,
            ThumbnailAlt = thumbnail?.ImageAlt,
            IsAvailable = entity.IsAvailable
        };
    }
}