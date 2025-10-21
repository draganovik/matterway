using Asp.Versioning;
using AutoMapper;
using Ordering.API.Features.Addresses.Contracts;
using Ordering.API.Features.Addresses.Data;
using Ordering.API.Features.Addresses.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Enums;
using Shared.Iterfaces;

namespace Ordering.API.Features.Addresses.Endpoints;

public class GetAddressById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Addresses/{id:guid}", Handler)
            .WithName("GetAddressById").WithSummary("Get Address by id.")
            .WithTags(nameof(Address))
            .Produces<AddressBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<AddressBaseResponse>, NotFound>>
        Handler(Guid id, IAddressRepository addressRepository, IMapper mapper)
    {
        var entity = await addressRepository.GetById(id);
        return entity is not null
            ? TypedResults.Ok(mapper.Map<AddressBaseResponse>(entity))
            : TypedResults.NotFound();
    }
}