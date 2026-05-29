using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Self.Customers.Endpoints;

public class SelfUpdateCustomer : IEndpoint
{
    private const string RouteName = nameof(SelfUpdateCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Self, "profile", Handler)
            .WithName(RouteName).WithSummary("[self] Update own Customer profile.")
            .WithTags(nameof(Customer))
            .Produces<SelfBaseCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<SelfBaseCustomerResponse>, NotFound, ForbidHttpResult>> Handler(
        SelfUpdateCustomerRequest request,
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        if (!RequestIdentity.CanManageOwnedResource(httpContext.User, request.SystemUserId))
            return TypedResults.Forbid();

        var entity = await customerRepository.GetBy(request.SystemUserId, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        entity.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.BirthDate,
            request.DefaultAddressId);

        var updated = await customerRepository.Update(entity, cancellationToken);
        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(updated));
    }

    private static SelfBaseCustomerResponse ToResponse(Customer entity)
    {
        return new SelfBaseCustomerResponse
        {
            SystemUserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}