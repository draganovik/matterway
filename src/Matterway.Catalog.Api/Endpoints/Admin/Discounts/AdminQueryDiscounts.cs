using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.Discounts;

public class AdminQueryDiscounts : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "discounts", Handle)
            .WithName("AdminQueryDiscounts").WithSummary("[admin] Query discount rows")
            .WithTags(nameof(Discount))
            .Produces<ICollection<QueryDiscountResponse>>()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Ok<ICollection<QueryDiscountResponse>>> Handle(
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken)
    {
        var entities = await discountRepository.Query(cancellationToken);
        var response = entities.Select(MapToResponse).ToList();
        return TypedResults.Ok<ICollection<QueryDiscountResponse>>(response);
    }

    public record QueryDiscountResponse
    {
        public string? Code { get; init; }
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
        public Guid ArticleId { get; init; }
    }

    public static QueryDiscountResponse MapToResponse(Discount entity)
    {
        return new QueryDiscountResponse
        {
            Code = entity.Code,
            Percentage = entity.Percentage,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
            ArticleId = entity.ArticleId
        };
    }
}