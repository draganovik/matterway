using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.Discounts.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

namespace Matterway.Catalog.Api.Features.Admin.Discounts.Endpoints;

public class AdminUpdateDiscount : IEndpoint
{
    private const string RouteName = nameof(AdminUpdateDiscount);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Admin, "discounts/{code}", Handle)
            .WithName(RouteName).WithSummary("[admin] Create or replace a discount across article codes")
            .WithTags(nameof(Discount))
            .Produces<AdminUpdateDiscountResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminUpdateDiscountResponse>, BadRequest<ProblemDetails>>> Handle(
        string code,
        AdminUpdateDiscountRequest request,
        IArticleRepository articleRepository,
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalizedCode))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Code is required",
                Status = StatusCodes.Status400BadRequest
            });

        var validFrom = request.ValidFrom.ToUniversalTime();
        var validTo = request.ValidTo?.ToUniversalTime();

        if (validTo.HasValue && validTo.Value < validFrom)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid validity range",
                Status = StatusCodes.Status400BadRequest,
                Detail = "ValidTo must be greater than or equal to ValidFrom."
            });

        var articleCodes = request.ArticleCodes?
            .Select(static code => ArticleCode.Parse(code, null))
            .ToArray() ?? [];

        if (articleCodes.Length == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot update discounts",
                Status = StatusCodes.Status400BadRequest,
                Detail = "At least one articleCode is required."
            });

        var articles = await articleRepository.GetByCodes(articleCodes, cancellationToken);
        var distinctCodes = articleCodes.Distinct().ToArray();
        if (articles.Count != distinctCodes.Length)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot update discounts",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more article codes do not exist."
            });

        var newDiscounts = MapToEntities(normalizedCode, request, articles, validFrom, validTo).ToList();

        IReadOnlyCollection<Discount> updated;
        try
        {
            updated = await discountRepository.Update(normalizedCode, newDiscounts, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot update discounts",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }

        return TypedResults.Ok(ToResponse(updated.First()));
    }

    private static IEnumerable<Discount> MapToEntities(
        string code,
        AdminUpdateDiscountRequest request,
        IEnumerable<Article> articles,
        DateTime validFromUtc,
        DateTime? validToUtc)
    {
        return articles
            .Select(article => new Discount
            {
                Code = code,
                Percentage = request.Percentage,
                ValidFrom = validFromUtc,
                ValidTo = validToUtc,
                ArticleCode = article.ArticleCode,
                Article = article
            });
    }

    private static AdminUpdateDiscountResponse ToResponse(Discount entity)
    {
        return new AdminUpdateDiscountResponse
        {
            Code = entity.Code,
            Percentage = entity.Percentage,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
            ArticleCode = ArticleCode.Parse(entity.ArticleCode, null)
        };
    }
}