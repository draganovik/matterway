using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.ArticleDetails.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails.Endpoints;

public sealed class AdminPutArticleDetail : IEndpoint
{
    private const string RouteName = nameof(AdminPutArticleDetail);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Admin, "articles/{article:ArticleCode}/details/{slug}", Handle)
            .WithName(RouteName)
            .WithSummary("[admin] Create or replace an ArticleDetail")
            .WithTags("ArticleDetail")
            .Produces<AdminPutArticleDetailResponse>()
            .Produces<AdminPutArticleDetailResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminPutArticleDetailResponse>, Created<AdminPutArticleDetailResponse>,
            NotFound, BadRequest<ProblemDetails>>>
        Handle(
            ArticleCode article,
            string slug,
            AdminPutArticleDetailRequest request,
            HttpContext httpContext,
            IArticleRepository articleRepository,
            IDetailRepository detailRepository,
            IArticleDetailTextRepository detailTextRepository,
            IArticleDetailNumericRepository detailNumericRepository,
            CancellationToken cancellationToken)
    {
        var articleEntity = await articleRepository.GetBy(article, cancellationToken);
        if (articleEntity is null) return TypedResults.NotFound();

        var normalizedSlug = slug.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedSlug))
            return BadRequestProblem("Detail slug is required.");

        var hasText = !string.IsNullOrWhiteSpace(request.TextValue);
        var hasNumeric = request.NumericValue is not null;
        if (hasText == hasNumeric)
            return BadRequestProblem("Provide exactly one of textValue or numericValue.");

        var detail = await detailRepository.GetBy(normalizedSlug, cancellationToken);
        if (detail is null)
            return BadRequestProblem("Detail definition not found.");

        var isNumeric = !string.IsNullOrWhiteSpace(detail.Unit);
        if (isNumeric != hasNumeric)
            return BadRequestProblem($"Detail '{detail.Slug}' requires a {(isNumeric ? "numeric" : "text")} value.");

        return isNumeric
            ? await PutNumeric(
                article,
                articleEntity.Title,
                normalizedSlug,
                request.NumericValue!.Value,
                detailTextRepository,
                detailNumericRepository,
                httpContext,
                cancellationToken)
            : await PutText(
                article,
                articleEntity.Title,
                normalizedSlug,
                request.TextValue!.Trim(),
                detailTextRepository,
                detailNumericRepository,
                httpContext,
                cancellationToken);
    }

    private static async Task<Results<Ok<AdminPutArticleDetailResponse>, Created<AdminPutArticleDetailResponse>,
            NotFound, BadRequest<ProblemDetails>>>
        PutNumeric(
            ArticleCode article,
            string articleTitle,
            string detailSlug,
            decimal value,
            IArticleDetailTextRepository detailTextRepository,
            IArticleDetailNumericRepository detailNumericRepository,
            HttpContext httpContext,
            CancellationToken cancellationToken)
    {
        if (await detailTextRepository.GetBy(article, detailSlug, cancellationToken) is not null)
            return BadRequestProblem("Stored article detail type does not match its definition.");

        var existing = await detailNumericRepository.GetBy(article, detailSlug, cancellationToken);
        ArticleDetailNumeric? saved;
        if (existing is null)
        {
            saved = await detailNumericRepository.Create(new ArticleDetailNumeric
            {
                ArticleCode = article.ToString(),
                DetailSlug = detailSlug,
                Value = value
            }, cancellationToken);
        }
        else
        {
            existing.Value = value;
            saved = await detailNumericRepository.Update(article, detailSlug, existing, cancellationToken);
        }

        if (saved is null) return BadRequestProblem("Article detail could not be stored.");

        var response = ToResponse(saved, article, articleTitle);
        return existing is null
            ? TypedResults.Created(httpContext.Request.Path, response)
            : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<AdminPutArticleDetailResponse>, Created<AdminPutArticleDetailResponse>,
            NotFound, BadRequest<ProblemDetails>>>
        PutText(
            ArticleCode article,
            string articleTitle,
            string detailSlug,
            string value,
            IArticleDetailTextRepository detailTextRepository,
            IArticleDetailNumericRepository detailNumericRepository,
            HttpContext httpContext,
            CancellationToken cancellationToken)
    {
        if (await detailNumericRepository.GetBy(article, detailSlug, cancellationToken) is not null)
            return BadRequestProblem("Stored article detail type does not match its definition.");

        var existing = await detailTextRepository.GetBy(article, detailSlug, cancellationToken);
        ArticleDetailText? saved;
        if (existing is null)
        {
            saved = await detailTextRepository.Create(new ArticleDetailText
            {
                ArticleCode = article.ToString(),
                DetailSlug = detailSlug,
                Value = value
            }, cancellationToken);
        }
        else
        {
            existing.Value = value;
            saved = await detailTextRepository.Update(article, detailSlug, existing, cancellationToken);
        }

        if (saved is null) return BadRequestProblem("Article detail could not be stored.");

        var response = ToResponse(saved, article, articleTitle);
        return existing is null
            ? TypedResults.Created(httpContext.Request.Path, response)
            : TypedResults.Ok(response);
    }

    private static BadRequest<ProblemDetails> BadRequestProblem(string detail)
    {
        return TypedResults.BadRequest(new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail
        });
    }

    private static AdminPutArticleDetailResponse ToResponse(
        ArticleDetailText entity,
        ArticleCode article,
        string articleTitle)
    {
        return new AdminPutArticleDetailResponse
        {
            ArticleCode = article,
            ArticleTitle = articleTitle,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            TextValue = entity.Value
        };
    }

    private static AdminPutArticleDetailResponse ToResponse(
        ArticleDetailNumeric entity,
        ArticleCode article,
        string articleTitle)
    {
        return new AdminPutArticleDetailResponse
        {
            ArticleCode = article,
            ArticleTitle = articleTitle,
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Unit = entity.Detail?.Unit,
            NumericValue = entity.Value
        };
    }
}
