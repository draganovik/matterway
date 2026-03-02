using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.ArticleDetails;

public class AdminUpdateArticleDetail : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "articles/{articleId:guid}/details/{detailSlug}", Handle)
            .WithName("AdminUpdateArticleDetail").WithSummary("[admin] Update an ArticleDetail")
            .WithTags("ArticleDetail")
            .Produces<UpdateArticleDetailResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateArticleDetailResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handle(
            Guid articleId,
            string detailSlug,
            UpdateArticleDetailRequest request,
            IDetailRepository detailRepository,
            IArticleDetailTextRepository detailTextRepository,
            IArticleDetailNumericRepository detailNumericRepository,
            CancellationToken cancellationToken)
    {
        var normalizedSlug = detailSlug.Trim().ToLower();
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

        if (isNumeric)
        {
            var entity = await detailNumericRepository.GetBy(articleId, normalizedSlug, cancellationToken);
            if (entity is null) return TypedResults.NotFound();
            entity.Value = request.NumericValue!.Value;
            var updated = await detailNumericRepository.Update(articleId, normalizedSlug, entity, cancellationToken);
            if (updated is null) return TypedResults.NotFound();
            return TypedResults.Ok(MapToResponse(updated));
        }

        var textEntity = await detailTextRepository.GetBy(articleId, normalizedSlug, cancellationToken);
        if (textEntity is null) return TypedResults.NotFound();
        textEntity.Value = request.TextValue!.Trim();
        var savedText = await detailTextRepository.Update(articleId, normalizedSlug, textEntity, cancellationToken);
        if (savedText is null) return TypedResults.NotFound();
        return TypedResults.Ok(MapToResponse(savedText));
    }

    public record UpdateArticleDetailRequest
    {
        public string? TextValue { get; init; }
        public decimal? NumericValue { get; init; }
    }

    public record UpdateArticleDetailResponse
    {
        public Guid ArticleId { get; init; }
        public string? ArticleTitle { get; init; }
        public string? DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
        public string? TextValue { get; init; }
        public decimal? NumericValue { get; init; }
    }

    private static BadRequest<ProblemDetails>? ValidateRequest(UpdateArticleDetailRequest request)
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

    private static ProblemDetails BuildValueMismatchProblem(Detail detail, string expectedType)
    {
        return new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Detail '{detail.Slug}' requires a {expectedType} value."
        };
    }

    public static UpdateArticleDetailResponse MapToResponse(ArticleDetailText entity)
    {
        return new UpdateArticleDetailResponse
        {
            ArticleId = entity.ArticleId,
            ArticleTitle = entity.Article?.Title,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = entity.Value,
            NumericValue = null
        };
    }

    public static UpdateArticleDetailResponse MapToResponse(ArticleDetailNumeric entity)
    {
        return new UpdateArticleDetailResponse
        {
            ArticleId = entity.ArticleId,
            ArticleTitle = entity.Article?.Title,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = null,
            NumericValue = entity.Value
        };
    }
}