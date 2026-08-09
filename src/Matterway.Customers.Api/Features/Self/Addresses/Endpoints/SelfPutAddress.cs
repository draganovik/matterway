using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.Addresses.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Self.Addresses.Endpoints;

public class SelfPutAddress : IEndpoint
{
    private const string RouteName = nameof(SelfPutAddress);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Self, "address", Handler)
            .WithName(RouteName).WithSummary("[self] Create or update own Address.")
            .WithTags(nameof(Address))
            .Produces<SelfBaseAddressResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<
            Results<Ok<SelfBaseAddressResponse>, BadRequest<ProblemDetails>, NotFound, ForbidHttpResult>>
        Handler(
            SelfPutAddressRequest request,
            HttpContext httpContext,
            ICustomerRepository customerRepository,
            IAddressRepository addressRepository,
            CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.Forbid();

        var customer = await customerRepository.GetBy(customerId.Value, cancellationToken);
        if (customer is null) return TypedResults.NotFound();

        var address = await addressRepository.GetByCustomerId(customer.Id, cancellationToken);

        if (address is null)
            address = ToEntity(Guid.NewGuid(), customer.Id, request);
        else
            ApplyToEntity(address, request);

        var saved = (await addressRepository.Upsert(address, cancellationToken))!;

        if (customer.DefaultAddressId != saved.Id)
        {
            customer.DefaultAddressId = saved.Id;
            var updatedCustomer = await customerRepository.Update(customer, cancellationToken);
            if (updatedCustomer is null)
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Cannot update customer default address."
                });
        }

        return TypedResults.Ok(ToResponse(saved));
    }

    private static Address ToEntity(Guid addressId, Guid customerId, SelfPutAddressRequest request)
    {
        return new Address
        {
            Id = addressId,
            CustomerId = customerId,
            Country = request.Country,
            City = request.City,
            ZipCode = request.ZipCode,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            ContactPhone = request.ContactPhone
        };
    }

    private static void ApplyToEntity(Address entity, SelfPutAddressRequest request)
    {
        entity.Country = request.Country;
        entity.City = request.City;
        entity.ZipCode = request.ZipCode;
        entity.AddressLine1 = request.AddressLine1;
        entity.AddressLine2 = request.AddressLine2;
        entity.ContactPhone = request.ContactPhone;
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
