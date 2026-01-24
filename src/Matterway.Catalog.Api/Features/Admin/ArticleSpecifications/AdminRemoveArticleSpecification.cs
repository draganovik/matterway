using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleSpecificationEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Admin.ArticleSpecifications;

public class AdminRemoveArticleSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/articles/{articleId:guid}/specifications/{specificationSlug}", Handle)
            .WithName("AdminDeleteArticleSpecification").WithSummary("Delete an ArticleSpecification (admin).")
            .WithTags(nameof(ArticleSpecification))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RemoveArticleSpecificationResponse>, NotFound>> Handle(
        Guid articleId,
        string specificationSlug,
        IArticleSpecificationRepository repository,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetBy(articleId, specificationSlug, cancellationToken);

        if (entity is null) return TypedResults.NotFound();

        var isDeleted = await repository.Delete(articleId, specificationSlug, cancellationToken);

        return isDeleted ? TypedResults.Ok(MapToResponse(entity)) : TypedResults.NotFound();
    }

    public record RemoveArticleSpecificationResponse
    {
        public required Guid ArticleId { get; init; }
        public required string Specification { get; init; }
        public string Message { get; init; } = "Article specification removed successfully.";
    }

    public static RemoveArticleSpecificationResponse MapToResponse(ArticleSpecification entity)
    {
        return new RemoveArticleSpecificationResponse
        {
            ArticleId = entity.ArticleId,
            Specification = entity.Specification?.Title ?? "Specification key: " + entity.SpecificationSlug
        };
    }
}