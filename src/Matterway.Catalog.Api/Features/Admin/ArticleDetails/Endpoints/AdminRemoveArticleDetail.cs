using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.ArticleDetails.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;

namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails.Endpoints;

public class AdminRemoveArticleDetail : IEndpoint
{
    private const string RouteName = nameof(AdminRemoveArticleDetail);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "articles/{article:ArticleCode}/details/{detailSlug}", Handle)
            .WithName(RouteName).WithSummary("[admin] Delete an ArticleDetail")
            .WithTags("ArticleDetail")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminRemoveArticleDetailResponse>, NotFound>> Handle(
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
            return isDeleted ? TypedResults.Ok(ToResponse(textEntity, article)) : TypedResults.NotFound();
        }

        var numericEntity = await detailNumericRepository.GetBy(article, normalizedSlug, cancellationToken);
        if (numericEntity is null) return TypedResults.NotFound();
        var numericDeleted = await detailNumericRepository.Delete(article, normalizedSlug, cancellationToken);
        return numericDeleted ? TypedResults.Ok(ToResponse(numericEntity, article)) : TypedResults.NotFound();
    }

    private static AdminRemoveArticleDetailResponse ToResponse(ArticleDetailText entity, ArticleCode article)
    {
        return new AdminRemoveArticleDetailResponse
        {
            ArticleCode = article,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit
        };
    }

    private static AdminRemoveArticleDetailResponse ToResponse(ArticleDetailNumeric entity, ArticleCode article)
    {
        return new AdminRemoveArticleDetailResponse
        {
            ArticleCode = article,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit
        };
    }
}