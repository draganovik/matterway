using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ArticleDetails;

public class AddArticleDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Articles/{articleId:Guid}/Details", Handle)
            .WithName("AddArticleDetail").WithSummary("Add a new ArticleDetail.")
            .WithTags(nameof(ArticleDetail))
            .Produces<AddArticleDetailResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddArticleDetailResponse>, BadRequest<ProblemDetails>>> Handle(
        [FromRoute]
        Guid articleId,
        AddArticleDetailRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IArticleDetailRepository articleDetailRepository,
        CancellationToken cancellationToken)
    {
        var articleDetailModel = MapToEntity(articleId, request);
        var created = await articleDetailRepository.Create(articleDetailModel, cancellationToken);
        if (created is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = linkGenerator.GetUriByName(
            httpContext,
            "GetArticleById",
            new
            {
                id = created.ArticleId
            });

        return TypedResults.Created(location,
            MapToResponse(created));
    }

    public record AddArticleDetailRequest
    {
        [Required]
        public required string DetailSlug { get; init; }

        [Required]
        public required string Value { get; init; }
    }

    public record AddArticleDetailResponse
    {
        public Guid ArticleId { get; init; }
        public string? ArticleTitle { get; init; }
        public string? DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Value { get; init; }
    }

    public static ArticleDetail MapToEntity(Guid articleId, AddArticleDetailRequest request)
    {
        return new ArticleDetail
        {
            ArticleId = articleId,
            DetailSlug = request.DetailSlug,
            Value = request.Value
        };
    }

    public static AddArticleDetailResponse MapToResponse(ArticleDetail entity)
    {
        return new AddArticleDetailResponse
        {
            ArticleId = entity.ArticleId,
            ArticleTitle = entity.Article?.Title,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Value = entity.Value
        };
    }
}