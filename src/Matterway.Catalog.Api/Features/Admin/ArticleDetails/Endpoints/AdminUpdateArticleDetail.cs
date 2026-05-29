using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.ArticleDetails.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails.Endpoints;

public class AdminUpdateArticleDetail : IEndpoint
{
    private const string RouteName = nameof(AdminUpdateArticleDetail);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "articles/{article:ArticleCode}/details/{detailSlug}", Handle)
            .WithName(RouteName).WithSummary("[admin] Update an ArticleDetail")
            .WithTags("ArticleDetail")
            .Produces<AdminUpdateArticleDetailResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminUpdateArticleDetailResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handle(
            ArticleCode article,
            string detailSlug,
            AdminUpdateArticleDetailRequest request,
            IArticleRepository articleRepository,
            IDetailRepository detailRepository,
            IArticleDetailTextRepository detailTextRepository,
            IArticleDetailNumericRepository detailNumericRepository,
            CancellationToken cancellationToken)
    {
        var articleEntity = await articleRepository.GetBy(article, cancellationToken);
        if (articleEntity is null) return TypedResults.NotFound();

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
            var entity = await detailNumericRepository.GetBy(article, normalizedSlug, cancellationToken);
            if (entity is null) return TypedResults.NotFound();
            entity.Value = request.NumericValue!.Value;
            var updated = await detailNumericRepository.Update(article, normalizedSlug, entity, cancellationToken);
            if (updated is null) return TypedResults.NotFound();
            return TypedResults.Ok(ToResponse(updated, article, articleEntity.Title));
        }

        var textEntity = await detailTextRepository.GetBy(article, normalizedSlug, cancellationToken);
        if (textEntity is null) return TypedResults.NotFound();
        textEntity.Value = request.TextValue!.Trim();
        var savedText = await detailTextRepository.Update(article, normalizedSlug, textEntity, cancellationToken);
        if (savedText is null) return TypedResults.NotFound();
        return TypedResults.Ok(ToResponse(savedText, article, articleEntity.Title));
    }

    private static BadRequest<ProblemDetails>? ValidateRequest(AdminUpdateArticleDetailRequest request)
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

    private static AdminUpdateArticleDetailResponse ToResponse(
        ArticleDetailText entity,
        ArticleCode article,
        string articleTitle)
    {
        return new AdminUpdateArticleDetailResponse
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

    private static AdminUpdateArticleDetailResponse ToResponse(
        ArticleDetailNumeric entity,
        ArticleCode article,
        string articleTitle)
    {
        return new AdminUpdateArticleDetailResponse
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
