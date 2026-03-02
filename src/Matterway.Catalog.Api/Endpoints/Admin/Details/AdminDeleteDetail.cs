using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.Details;

public class AdminDeleteDetail : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "details/{slug}", Handle)
            .WithName("AdminDeleteDetail").WithSummary("[admin] Delete a Detail definition")
            .WithTags(nameof(Detail))
            .Produces<DeleteDetailResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteDetailResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
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

        return TypedResults.Ok(new DeleteDetailResponse
        {
            Slug = normalizedSlug,
            Message = "Detail removed successfully."
        });
    }

    public record DeleteDetailResponse
    {
        public string Slug { get; init; } = default!;
        public string Message { get; init; } = "Detail removed successfully.";
    }
}