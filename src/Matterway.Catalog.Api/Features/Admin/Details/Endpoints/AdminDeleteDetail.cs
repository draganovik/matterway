using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.Details.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Features.Admin.Details.Endpoints;

public class AdminDeleteDetail : IEndpoint
{
    private const string RouteName = nameof(AdminDeleteDetail);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "details/{slug}", Handle)
            .WithName(RouteName).WithSummary("[admin] Delete a Detail definition")
            .WithTags(nameof(Detail))
            .Produces<AdminDeleteDetailResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminDeleteDetailResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        string slug,
        IDetailRepository repository,
        CancellationToken cancellationToken)
    {
        var normalizedSlug = slug.Trim().ToLower();
        var entity = await repository.GetBy(normalizedSlug, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        var deleted = await repository.Delete(normalizedSlug, cancellationToken);
        if (!deleted)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot delete detail",
                Detail = "Detail is referenced by one or more articles.",
                Status = StatusCodes.Status400BadRequest
            });

        return TypedResults.Ok(new AdminDeleteDetailResponse
        {
            Slug = normalizedSlug,
            Message = "Detail removed successfully."
        });
    }

}
