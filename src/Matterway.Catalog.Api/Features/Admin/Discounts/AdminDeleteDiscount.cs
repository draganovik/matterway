using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

namespace Matterway.Catalog.Api.Features.Admin.Discounts;

public class AdminDeleteDiscount : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "discounts/{code}", Handle)
            .WithName("AdminDeleteDiscount").WithSummary("[admin] Delete all discounts for a code")
            .WithTags(nameof(Discount))
            .Produces<DeleteDiscountResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteDiscountResponse>, NotFound>> Handle(
        string code,
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalizedCode)) return TypedResults.NotFound();

        var deletedCount = await discountRepository.Delete(normalizedCode, cancellationToken);

        if (deletedCount == 0) return TypedResults.NotFound();

        return TypedResults.Ok(new DeleteDiscountResponse
        {
            Code = normalizedCode,
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