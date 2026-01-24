using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleSpecificationEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Admin.ArticleSpecifications;

public class AdminAddArticleSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("admin/articles/{articleId:Guid}/specifications", Handle)
            .WithName("AdminAddArticleSpecification").WithSummary("Add a new ArticleSpecification (admin).")
            .WithTags(nameof(ArticleSpecification))
            .Produces<AddArticleSpecificationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddArticleSpecificationResponse>, BadRequest<ProblemDetails>>> Handle(
        [FromRoute]
        Guid articleId,
        AddArticleSpecificationRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IArticleSpecificationRepository repository,
        CancellationToken cancellationToken)
    {
        var model = MapToEntity(articleId, request);
        var created = await repository.Create(model, cancellationToken);
        if (created is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create specification"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = linkGenerator.GetUriByName(
            httpContext,
            "PublicGetArticleById",
            new
            {
                id = created.ArticleId
            });

        return TypedResults.Created(location, MapToResponse(created));
    }

    public record AddArticleSpecificationRequest
    {
        [Required]
        public required string SpecificationSlug { get; init; }

        [Required]
        public required decimal Value { get; init; }
    }

    public record AddArticleSpecificationResponse
    {
        public Guid ArticleId { get; init; }
        public string? ArticleTitle { get; init; }
        public string? SpecificationSlug { get; init; }
        public string? Title { get; init; }
        public decimal Value { get; init; }
        public string? Unit { get; init; }
    }

    public static ArticleSpecification MapToEntity(Guid articleId, AddArticleSpecificationRequest request)
    {
        return new ArticleSpecification
        {
            ArticleId = articleId,
            SpecificationSlug = request.SpecificationSlug,
            Value = request.Value
        };
    }

    public static AddArticleSpecificationResponse MapToResponse(ArticleSpecification entity)
    {
        return new AddArticleSpecificationResponse
        {
            ArticleId = entity.ArticleId,
            ArticleTitle = entity.Article?.Title,
            SpecificationSlug = entity.SpecificationSlug,
            Title = entity.Specification?.Title,
            Value = entity.Value,
            Unit = entity.Specification?.Unit
        };
    }
}