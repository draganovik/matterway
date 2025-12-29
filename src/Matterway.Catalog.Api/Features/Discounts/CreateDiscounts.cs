using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Discounts;

public class CreateDiscounts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Discounts", Handle)
            .WithName("CreateDiscounts").WithSummary("Create discount codes for multiple products.")
            .WithTags(nameof(Discount))
            .Produces<CreatedDiscountResponse[]>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<CreatedDiscountResponse[]>, BadRequest<ProblemDetails>>> Handle(
        CreateDiscountRequest request,
        IDiscountRepository discountRepository,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
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

        if (request.ProductIds is null || request.ProductIds.Count == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot create discounts",
                Status = StatusCodes.Status400BadRequest,
                Detail = "At least one productId is required."
            });

        var discounts = MapToEntities(request, validFrom, validTo).ToList();

        IReadOnlyCollection<Discount> created;
        try
        {
            created = await discountRepository.CreateBulk(discounts, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot create discounts",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }

        if (created.Count == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot create discounts",
                Status = StatusCodes.Status400BadRequest,
                Detail = "No discounts were created."
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "GetProductById",
            new
            {
                id = created.First().ProductId
            });

        var response = created.Select(MapToResponse).ToArray();

        return TypedResults.Created(location, response);
    }

    public record CreateDiscountRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Code must be between 3 and 50 characters.")]
        [RegularExpression(@"^[A-Z0-9_-]+$",
            ErrorMessage = "Code can use uppercase letters, numbers, hyphens or underscores.")]
        public required string Code { get; init; }

        [Range(0.01, 1, ErrorMessage = "Percentage must be between 0.01 and 1.")]
        public required decimal Percentage { get; init; }

        [Required]
        public required DateTime ValidFrom { get; init; }

        public DateTime? ValidTo { get; init; }

        public ESupportedCurrency Currency { get; init; } = ESupportedCurrency.RSD;

        [Required]
        [MinLength(1, ErrorMessage = "At least one productId is required.")]
        public required ICollection<Guid> ProductIds { get; init; }
    }

    public record CreatedDiscountResponse
    {
        public required string Code { get; init; }
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
        public Guid ProductId { get; init; }
        public ESupportedCurrency Currency { get; init; }
    }

    private static IEnumerable<Discount> MapToEntities(CreateDiscountRequest request, DateTime validFromUtc,
        DateTime? validToUtc)
    {
        return request.ProductIds
            .Distinct()
            .Select(productId => new Discount
            {
                Code = request.Code,
                Percentage = request.Percentage,
                ValidFrom = validFromUtc,
                ValidTo = validToUtc,
                ProductId = productId,
                Currency = request.Currency
            });
    }

    private static CreatedDiscountResponse MapToResponse(Discount discount)
    {
        return new CreatedDiscountResponse
        {
            Code = discount.Code,
            Percentage = discount.Percentage,
            ValidFrom = discount.ValidFrom,
            ValidTo = discount.ValidTo,
            ProductId = discount.ProductId,
            Currency = discount.Currency
        };
    }
}