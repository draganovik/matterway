using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Admin.Addresses.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;

namespace Matterway.Customers.Api.Features.Admin.Addresses.Endpoints;

public class AdminGetAddressByCustomer : IEndpoint
{
    private const string RouteName = nameof(AdminGetAddressByCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "customers/{customerId:guid}/address", Handler)
            .WithName(RouteName).WithSummary("[admin] Get Customer address by customer id")
            .WithTags(nameof(Address))
            .Produces<AdminBaseAddressResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminBaseAddressResponse>, NotFound>> Handler(
        Guid customerId,
        IAddressRepository addressRepository,
        CancellationToken cancellationToken)
    {
        var entity = await addressRepository.GetByCustomerId(customerId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(entity));
    }

    private static AdminBaseAddressResponse ToResponse(Address entity)
    {
        return new AdminBaseAddressResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Country = entity.Country,
            City = entity.City,
            ZipCode = entity.ZipCode,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            ContactPhone = entity.ContactPhone
        };
    }
}
