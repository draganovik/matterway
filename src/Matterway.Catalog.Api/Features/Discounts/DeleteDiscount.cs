using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntityDiscount;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Discounts;

public class DeleteDiscount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Discounts/{code}", Handle)
            .WithName("DeleteDiscount").WithSummary("Delete all discounts for a code.")
            .WithTags(nameof(Discount))
            .Produces<DeleteDiscountResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteDiscountResponse>, NotFound>> Handle(
        string code,
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken)
    {
        var deletedCount = await discountRepository.DeleteByCodeAsync(code, cancellationToken);

        if (deletedCount == 0) return TypedResults.NotFound();

        return TypedResults.Ok(new DeleteDiscountResponse
        {
            Code = code,
            RemovedCount = deletedCount
        });
    }

    public record DeleteDiscountResponse
    {
        public string Code { get; init; } = string.Empty;
        public int RemovedCount { get; init; }
        public string Message { get; init; } = "Discount(s) removed successfully.";
    }
}