using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.Discounts.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

namespace Matterway.Catalog.Api.Features.Admin.Discounts.Endpoints;

public class AdminQueryDiscounts : IEndpoint
{
    private const string RouteName = nameof(AdminQueryDiscounts);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "discounts", Handle)
            .WithName(RouteName).WithSummary("[admin] Query discount rows")
            .WithTags(nameof(Discount))
            .Produces<ICollection<AdminQueryDiscountResponse>>()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Ok<ICollection<AdminQueryDiscountResponse>>> Handle(
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken)
    {
        var entities = await discountRepository.Query(cancellationToken);
        var response = entities.Select(ToResponse).ToList();
        return TypedResults.Ok<ICollection<AdminQueryDiscountResponse>>(response);
    }

    private static AdminQueryDiscountResponse ToResponse(Discount entity)
    {
        return new AdminQueryDiscountResponse
        {
            Code = entity.Code,
            Percentage = entity.Percentage,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
            ArticleCode = ArticleCode.Parse(entity.ArticleCode, null)
        };
    }
}
