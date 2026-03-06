using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.ArticleDetails;

public class AdminAddArticleDetail : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "articles/{article:ArticleCode}/details", Handle)
            .WithName("AdminAddArticleDetail").WithSummary("[admin] Add a new ArticleDetail")
            .WithTags("ArticleDetail")
            .Produces<AddArticleDetailResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<AddArticleDetailResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        [FromRoute]
        ArticleCode article,
        AddArticleDetailRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IArticleRepository articleRepository,
        IDetailRepository detailRepository,
        IArticleDetailTextRepository detailTextRepository,
        IArticleDetailNumericRepository detailNumericRepository,
        CancellationToken cancellationToken)
    {
        var articleEntity = await articleRepository.GetBy(article, cancellationToken);
        if (articleEntity is null) return TypedResults.NotFound();

        var normalizedSlug = request.DetailSlug.Trim().ToLower();
        var validation = ValidateRequest(request);
        if (validation is not null) return validation;

        var detail = await detailRepository.GetBy(normalizedSlug, cancellationToken);
        if (detail is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Detail definition not found."
            });

        var isNumeric = !string.IsNullOrWhiteSpace(detail.Unit);
        if (isNumeric && request.NumericValue is null)
            return TypedResults.BadRequest(BuildValueMismatchProblem(detail, "numeric"));
        if (!isNumeric && string.IsNullOrWhiteSpace(request.TextValue))
            return TypedResults.BadRequest(BuildValueMismatchProblem(detail, "text"));

        var existingText = await detailTextRepository.GetBy(article, normalizedSlug, cancellationToken);
        var existingNumeric = await detailNumericRepository.GetBy(article, normalizedSlug, cancellationToken);
        if (existingText is not null || existingNumeric is not null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Article detail already exists."
            });

        AddArticleDetailResponse created;
        if (isNumeric)
        {
            var entity = new ArticleDetailNumeric
            {
                ArticleCode = article.ToString(),
                DetailSlug = normalizedSlug,
                Value = request.NumericValue!.Value
            };

            var saved = await detailNumericRepository.Create(entity, cancellationToken);
            if (saved is null) return TypedResults.BadRequest(BuildCreateProblem());
            created = MapToResponse(saved, article, articleEntity.Title);
        }
        else
        {
            var entity = new ArticleDetailText
            {
                ArticleCode = article.ToString(),
                DetailSlug = normalizedSlug,
                Value = request.TextValue!.Trim()
            };

            var saved = await detailTextRepository.Create(entity, cancellationToken);
            if (saved is null) return TypedResults.BadRequest(BuildCreateProblem());
            created = MapToResponse(saved, article, articleEntity.Title);
        }

        var location = linkGenerator.GetUriByName(
            httpContext,
            "PublicGetArticleByCode",
            new
            {
                code = created.ArticleCode
            });

        return TypedResults.Created(location, created);
    }

    public record AddArticleDetailRequest
    {
        [Required]
        public required string DetailSlug { get; init; }

        public string? TextValue { get; init; }
        public decimal? NumericValue { get; init; }
    }

    public record AddArticleDetailResponse
    {
        public required ArticleCode ArticleCode { get; init; }
        public string? ArticleTitle { get; init; }
        public string? DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
        public string? TextValue { get; init; }
        public decimal? NumericValue { get; init; }
    }

    private static BadRequest<ProblemDetails>? ValidateRequest(AddArticleDetailRequest request)
    {
        var hasText = !string.IsNullOrWhiteSpace(request.TextValue);
        var hasNumeric = request.NumericValue is not null;
        if (hasText == hasNumeric)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Provide exactly one of textValue or numericValue."
            });

        return null;
    }

    private static ProblemDetails BuildCreateProblem()
    {
        return new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Cannot create entity"
        };
    }

    private static ProblemDetails BuildValueMismatchProblem(Detail detail, string expectedType)
    {
        return new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Detail '{detail.Slug}' requires a {expectedType} value."
        };
    }

    public static AddArticleDetailResponse MapToResponse(
        ArticleDetailText entity,
        ArticleCode article,
        string articleTitle)
    {
        return new AddArticleDetailResponse
        {
            ArticleCode = article,
            ArticleTitle = articleTitle,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = entity.Value,
            NumericValue = null
        };
    }

    public static AddArticleDetailResponse MapToResponse(
        ArticleDetailNumeric entity,
        ArticleCode article,
        string articleTitle)
    {
        return new AddArticleDetailResponse
        {
            ArticleCode = article,
            ArticleTitle = articleTitle,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = null,
            NumericValue = entity.Value
        };
    }
}