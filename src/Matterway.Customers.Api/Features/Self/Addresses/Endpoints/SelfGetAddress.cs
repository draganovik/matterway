using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.Addresses.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Self.Addresses.Endpoints;

public class SelfGetAddress : IEndpoint
{
    private const string RouteName = nameof(SelfGetAddress);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "address", Handler)
            .WithName(RouteName).WithSummary("[self] Get own Address.")
            .WithTags(nameof(Address))
            .Produces<SelfBaseAddressResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<SelfBaseAddressResponse>, NotFound, ForbidHttpResult>> Handler(
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        IAddressRepository addressRepository,
        CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null)
            return TypedResults.Forbid();

        var customer = await customerRepository.GetBy(customerId.Value, cancellationToken);
        if (customer?.DefaultAddressId is null)
            return TypedResults.NotFound();

        var address = await addressRepository.GetByCustomerId(customer.Id, cancellationToken);
        if (address is null) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(address));
    }

    private static SelfBaseAddressResponse ToResponse(Address entity)
    {
        return new SelfBaseAddressResponse
        {
            Id = entity.Id,
            Country = entity.Country,
            City = entity.City,
            ZipCode = entity.ZipCode,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            ContactPhone = entity.ContactPhone
        };
    }
}