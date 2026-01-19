using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Discounts;

public class UpdateDiscount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("Discounts/{code}", Handle)
            .WithName("UpdateDiscount").WithSummary("Replace a discount across article ids.")
            .WithTags(nameof(Discount))
            .Produces<UpdatedDiscountResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdatedDiscountResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        string code,
        UpdateDiscountRequest request,
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken)
    {
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

        var existing = await discountRepository.GetBy(code, request.Currency, cancellationToken);
        if (existing.Count == 0) return TypedResults.NotFound();

        var newDiscounts = MapToEntities(code, request, validFrom, validTo).ToList();

        IReadOnlyCollection<Discount> updated;
        try
        {
            updated = await discountRepository.Update(code, request.Currency, newDiscounts, cancellationToken);
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

        if (updated.Count == 0) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated.First()));
    }

    public record UpdateDiscountRequest
    {
        [Range(0.01, 1, ErrorMessage = "Percentage must be between 0.01 and 1.")]
        public required decimal Percentage { get; init; }

        [Required]
        public required DateTime ValidFrom { get; init; }

        public DateTime? ValidTo { get; init; }

        public ESupportedCurrency Currency { get; init; } = ESupportedCurrency.RSD;

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
        public ESupportedCurrency Currency { get; init; }
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
                ArticleId = articleId,
                Currency = request.Currency
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
            ArticleId = entity.ArticleId,
            Currency = entity.Currency
        };
    }
}