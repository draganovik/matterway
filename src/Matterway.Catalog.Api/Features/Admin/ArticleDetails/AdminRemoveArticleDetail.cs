using Asp.Versioning;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails;

public class AdminRemoveArticleDetail : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "articles/{articleId:guid}/details/{detailSlug}", Handle)
            .WithName("AdminDeleteArticleDetail").WithSummary("[admin] Delete an ArticleDetail")
            .WithTags("ArticleDetail")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveArticleDetailResponse>, NotFound>> Handle(
        Guid articleId,
        string detailSlug,
        IArticleDetailTextRepository detailTextRepository,
        IArticleDetailNumericRepository detailNumericRepository,
        CancellationToken cancellationToken)
    {
        var normalizedSlug = detailSlug.Trim().ToLower();
        var textEntity = await detailTextRepository.GetBy(articleId, normalizedSlug, cancellationToken);
        if (textEntity is not null)
        {
            var isDeleted = await detailTextRepository.Delete(articleId, normalizedSlug, cancellationToken);
            return isDeleted ? TypedResults.Ok(MapToResponse(textEntity)) : TypedResults.NotFound();
        }

        var numericEntity = await detailNumericRepository.GetBy(articleId, normalizedSlug, cancellationToken);
        if (numericEntity is null) return TypedResults.NotFound();
        var numericDeleted = await detailNumericRepository.Delete(articleId, normalizedSlug, cancellationToken);
        return numericDeleted ? TypedResults.Ok(MapToResponse(numericEntity)) : TypedResults.NotFound();
    }

    public record RemoveArticleDetailResponse
    {
        public required Guid ArticleId { get; init; }
        public required string DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
        public string Message { get; init; } = "Article detail removed successfully.";
    }

    public static RemoveArticleDetailResponse MapToResponse(ArticleDetailText entity)
    {
        return new RemoveArticleDetailResponse
        {
            ArticleId = entity.ArticleId,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit
        };
    }

    public static RemoveArticleDetailResponse MapToResponse(ArticleDetailNumeric entity)
    {
        return new RemoveArticleDetailResponse
        {
            ArticleId = entity.ArticleId,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit
        };
    }
}