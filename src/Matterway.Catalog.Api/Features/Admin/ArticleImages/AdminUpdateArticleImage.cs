using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;

namespace Matterway.Catalog.Api.Features.Admin.ArticleImages;

public class AdminUpdateArticleImage : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "articles/{articleId:guid}/images/{orderIndex:int}", Handle)
            .WithName("AdminUpdateArticleImage").WithSummary("[admin] Update an ArticleImage")
            .WithTags(nameof(ArticleImage))
            .Produces<UpdateArticleImageResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateArticleImageResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        Guid articleId,
        int orderIndex,
        UpdateArticleImageRequest request,
        IArticleImageRepository articleImageRepository,
        CancellationToken cancellationToken)
    {
        var entity = await articleImageRepository.GetBy(articleId, orderIndex, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdates(entity, request);

        var targetOrderIndex = request.OrderIndex ?? entity.OrderIndex;

        if (targetOrderIndex < 0) targetOrderIndex = 0;

        var updated = await articleImageRepository.Update(entity, targetOrderIndex, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateArticleImageRequest
    {
        public string? ImageAlt { get; init; }

        [Range(0, int.MaxValue)]
        public int? OrderIndex { get; init; }
    }

    public record UpdateArticleImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public Guid ArticleId { get; init; }
        public string? ArticleName { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }

    public static void MapUpdates(ArticleImage entity, UpdateArticleImageRequest request)
    {
        entity.ImageAlt = request.ImageAlt ?? entity.ImageAlt;
    }

    public static UpdateArticleImageResponse MapToResponse(ArticleImage entity)
    {
        return new UpdateArticleImageResponse
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ArticleId = entity.ArticleId,
            ArticleName = entity.Article?.Title,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}