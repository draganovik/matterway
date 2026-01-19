using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ArticleDetails;

public class UpdateArticleDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Articles/{articleId:guid}/Details/{detailSlug}", Handle)
            .WithName("UpdateArticleDetail").WithSummary("Update an ArticleDetail.")
            .WithTags(nameof(ArticleDetail))
            .Produces<UpdateArticleDetailResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateArticleDetailResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handle(
            Guid articleId,
            string detailSlug,
            UpdateArticleDetailRequest request,
            IArticleDetailRepository articleDetailRepository,
            CancellationToken cancellationToken)
    {
        var entity = await articleDetailRepository.GetBy(articleId, detailSlug, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdates(entity, request);

        var updated = await articleDetailRepository.Update(articleId, detailSlug, entity, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateArticleDetailRequest
    {
        [Required]
        public required string Value { get; init; }
    }

    public record UpdateArticleDetailResponse
    {
        public string? ArticleTitle { get; init; }
        public string? Detail { get; init; }
        public string? Value { get; init; }
    }

    public static void MapUpdates(ArticleDetail entity, UpdateArticleDetailRequest request)
    {
        entity.Value = request.Value;
    }

    public static UpdateArticleDetailResponse MapToResponse(ArticleDetail entity)
    {
        return new UpdateArticleDetailResponse
        {
            ArticleTitle = entity.Article?.Title,
            Detail = entity.Detail?.Title,
            Value = entity.Value
        };
    }
}