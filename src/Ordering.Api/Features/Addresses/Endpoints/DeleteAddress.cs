using Asp.Versioning;
using Ordering.Api.Features.Addresses.Data;
using Ordering.Api.Features.Addresses.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Abstractions;

namespace Ordering.Api.Features.Addresses.Endpoints;

public class DeleteAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Addresses/{id:guid}", Handler)
            .WithName("DeleteAddress").WithSummary("Delete Address by id.")
            .WithTags(nameof(Address))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(Guid id, IAddressRepository addressRepository)
    {
        var isDeleted = await addressRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}