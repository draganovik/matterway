using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Admin.Discounts;

public class AdminDeleteDiscount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/discounts/{code}", Handle)
            .WithName("AdminDeleteDiscount").WithSummary("Delete all discounts for a code (admin).")
            .WithTags(nameof(Discount))
            .Produces<DeleteDiscountResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteDiscountResponse>, NotFound>> Handle(
        string code,
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken)
    {
        var deletedCount = await discountRepository.Delete(code, cancellationToken);

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