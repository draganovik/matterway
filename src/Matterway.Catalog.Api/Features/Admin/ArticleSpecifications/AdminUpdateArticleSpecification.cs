using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleSpecificationEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Admin.ArticleSpecifications;

public class AdminUpdateArticleSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/articles/{articleId:guid}/specifications/{specificationSlug}", Handle)
            .WithName("AdminUpdateArticleSpecification").WithSummary("Update an ArticleSpecification (admin).")
            .WithTags(nameof(ArticleSpecification))
            .Produces<UpdateArticleSpecificationResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateArticleSpecificationResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handle(
            Guid articleId,
            string specificationSlug,
            UpdateArticleSpecificationRequest request,
            IArticleSpecificationRepository repository,
            CancellationToken cancellationToken)
    {
        var entity = await repository.GetBy(articleId, specificationSlug, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdates(entity, request);

        var updated = await repository.Update(articleId, specificationSlug, entity, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateArticleSpecificationRequest
    {
        [Required]
        public required decimal Value { get; init; }
    }

    public record UpdateArticleSpecificationResponse
    {
        public string? ArticleTitle { get; init; }
        public string? Specification { get; init; }
        public decimal Value { get; init; }
        public string? Unit { get; init; }
    }

    public static void MapUpdates(ArticleSpecification entity, UpdateArticleSpecificationRequest request)
    {
        entity.Value = request.Value;
    }

    public static UpdateArticleSpecificationResponse MapToResponse(ArticleSpecification entity)
    {
        return new UpdateArticleSpecificationResponse
        {
            ArticleTitle = entity.Article?.Title,
            Specification = entity.Specification?.Title,
            Value = entity.Value,
            Unit = entity.Specification?.Unit
        };
    }
}