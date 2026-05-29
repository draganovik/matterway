using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.Discounts.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

namespace Matterway.Catalog.Api.Features.Admin.Discounts.Endpoints;

public class AdminDeleteDiscount : IEndpoint
{
    private const string RouteName = nameof(AdminDeleteDiscount);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "discounts/{code}", Handle)
            .WithName(RouteName).WithSummary("[admin] Delete all discounts for a code")
            .WithTags(nameof(Discount))
            .Produces<AdminDeleteDiscountResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminDeleteDiscountResponse>, NotFound>> Handle(
        string code,
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalizedCode)) return TypedResults.NotFound();

        var deletedCount = await discountRepository.Delete(normalizedCode, cancellationToken);

        if (deletedCount == 0) return TypedResults.NotFound();

        return TypedResults.Ok(new AdminDeleteDiscountResponse
        {
            Code = normalizedCode,
            RemovedCount = deletedCount
        });
    }
}