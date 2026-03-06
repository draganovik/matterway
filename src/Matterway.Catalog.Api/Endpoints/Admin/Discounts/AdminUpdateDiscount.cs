using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.Discounts;

public class AdminUpdateDiscount : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Admin, "discounts/{code}", Handle)
            .WithName("AdminUpdateDiscount").WithSummary("[admin] Create or replace a discount across article ids")
            .WithTags(nameof(Discount))
            .Produces<UpdatedDiscountResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<UpdatedDiscountResponse>, BadRequest<ProblemDetails>>> Handle(
        string code,
        UpdateDiscountRequest request,
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

        if (request.ArticleIds.Count == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot update discounts",
                Status = StatusCodes.Status400BadRequest,
                Detail = "At least one articleId is required."
            });

        var newDiscounts = MapToEntities(normalizedCode, request, validFrom, validTo).ToList();

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

        return TypedResults.Ok(MapToResponse(updated.First()));
    }

    public record UpdateDiscountRequest
    {
        [Range(0.01, 1, ErrorMessage = "Percentage must be between 0.01 and 1.")]
        public required decimal Percentage { get; init; }

        [Required]
        public required DateTime ValidFrom { get; init; }

        public DateTime? ValidTo { get; init; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one articleId is required.")]
        public required ICollection<Guid> ArticleIds { get; init; }
    }

    public record UpdatedDiscountResponse
    {
        public required string Code { get; init; }
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
        public Guid ArticleId { get; init; }
    }

    private static IEnumerable<Discount> MapToEntities(string code, UpdateDiscountRequest request,
        DateTime validFromUtc,
        DateTime? validToUtc)
    {
        return request.ArticleIds
            .Distinct()
            .Select(articleId => new Discount
            {
                Code = code,
                Percentage = request.Percentage,
                ValidFrom = validFromUtc,
                ValidTo = validToUtc,
                ArticleId = articleId
            });
    }

    private static UpdatedDiscountResponse MapToResponse(Discount entity)
    {
        return new UpdatedDiscountResponse
        {
            Code = entity.Code,
            Percentage = entity.Percentage,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
            ArticleId = entity.ArticleId
        };
    }
}