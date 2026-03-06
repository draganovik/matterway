using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.ArticleDetails;

public class AdminRemoveArticleDetail : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "articles/{article:ArticleCode}/details/{detailSlug}", Handle)
            .WithName("AdminDeleteArticleDetail").WithSummary("[admin] Delete an ArticleDetail")
            .WithTags("ArticleDetail")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<RemoveArticleDetailResponse>, NotFound>> Handle(
        ArticleCode article,
        string detailSlug,
        IArticleRepository articleRepository,
        IArticleDetailTextRepository detailTextRepository,
        IArticleDetailNumericRepository detailNumericRepository,
        CancellationToken cancellationToken)
    {
        var articleEntity = await articleRepository.GetBy(article, cancellationToken);
        if (articleEntity is null) return TypedResults.NotFound();

        var normalizedSlug = detailSlug.Trim().ToLower();
        var textEntity = await detailTextRepository.GetBy(article, normalizedSlug, cancellationToken);
        if (textEntity is not null)
        {
            var isDeleted = await detailTextRepository.Delete(article, normalizedSlug, cancellationToken);
            return isDeleted ? TypedResults.Ok(MapToResponse(textEntity, article)) : TypedResults.NotFound();
        }

        var numericEntity = await detailNumericRepository.GetBy(article, normalizedSlug, cancellationToken);
        if (numericEntity is null) return TypedResults.NotFound();
        var numericDeleted = await detailNumericRepository.Delete(article, normalizedSlug, cancellationToken);
        return numericDeleted ? TypedResults.Ok(MapToResponse(numericEntity, article)) : TypedResults.NotFound();
    }

    public record RemoveArticleDetailResponse
    {
        public required ArticleCode ArticleCode { get; init; }
        public required string DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
        public string Message { get; init; } = "Article detail removed successfully.";
    }

    public static RemoveArticleDetailResponse MapToResponse(ArticleDetailText entity, ArticleCode article)
    {
        return new RemoveArticleDetailResponse
        {
            ArticleCode = article,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit
        };
    }

    public static RemoveArticleDetailResponse MapToResponse(ArticleDetailNumeric entity, ArticleCode article)
    {
        return new RemoveArticleDetailResponse
        {
            ArticleCode = article,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit
        };
    }
}