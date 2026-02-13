using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Admin.ArticleImages;

public class AdminRemoveArticleImage : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "articles/{articleId:guid}/images/{orderIndex:int}", Handle)
            .WithName("AdminRemoveArticleImage").WithSummary("[admin] Remove an ArticleImage")
            .WithTags(nameof(ArticleImage))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveArticleImageResponse>, NotFound>> Handle(
        Guid articleId,
        int orderIndex,
        IArticleImageRepository articleImageRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        var entity = await articleImageRepository.GetBy(articleId, orderIndex, cancellationToken);

        if (entity is null) return TypedResults.NotFound();

        var isDeleted = await articleImageRepository.Delete(articleId, orderIndex, cancellationToken);

        if (!isDeleted) return TypedResults.NotFound();

        await imageStorageService.DeleteAsync(entity.ArticleId, entity.Id, cancellationToken);
        return TypedResults.Ok(MapToResponse(entity));
    }

    public record RemoveArticleImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public required string ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
        public required Guid ArticleId { get; init; }
        public string Message { get; init; } = "Article image removed successfully.";
    }

    public static RemoveArticleImageResponse MapToResponse(ArticleImage entity)
    {
        return new RemoveArticleImageResponse
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ArticleId = entity.ArticleId,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}