using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.ArticleDetails;

public class RemoveArticleDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Articles/{articleId:guid}/Details/{detailSlug}", Handle)
            .WithName("DeleteArticleDetail").WithSummary("Delete an ArticleDetail.")
            .WithTags(nameof(ArticleDetail))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveArticleDetailResponse>, NotFound>> Handle(
        Guid articleId,
        string detailSlug,
        IArticleDetailRepository articleDetailRepository,
        CancellationToken cancellationToken)
    {
        var entity = await articleDetailRepository.GetBy(articleId, detailSlug, cancellationToken);

        if (entity is null) return TypedResults.NotFound();

        var isDeleted = await articleDetailRepository.Delete(articleId, detailSlug, cancellationToken);

        return isDeleted ? TypedResults.Ok(MapToResponse(entity)) : TypedResults.NotFound();
    }

    public record RemoveArticleDetailResponse
    {
        public required Guid ArticleId { get; init; }
        public required string DetailType { get; init; }
        public string Message { get; init; } = "Article detail removed successfully.";
    }

    public static RemoveArticleDetailResponse MapToResponse(ArticleDetail entity)
    {
        return new RemoveArticleDetailResponse
        {
            ArticleId = entity.ArticleId,
            DetailType = entity.Detail?.Title ?? "Detail key: " + entity.DetailSlug
        };
    }
}