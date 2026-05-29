using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Admin.Addresses.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Admin.Addresses.Endpoints;

public class AdminPutAddressByCustomer : IEndpoint
{
    private const string RouteName = nameof(AdminPutAddressByCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Admin, "customers/{customerId:guid}/address", Handler)
            .WithName(RouteName)
            .WithSummary("[admin] Create or replace Customer address by customer id")
            .WithTags(nameof(Address))
            .Produces<AdminBaseAddressResponse>(StatusCodes.Status200OK)
            .Produces<AdminBaseAddressResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async
        Task<Results<Ok<AdminBaseAddressResponse>, Created<AdminBaseAddressResponse>, NotFound,
            BadRequest<ProblemDetails>>>
        Handler(
            Guid customerId,
            AdminPutAddressRequest request,
            HttpContext httpContext,
            ICustomerRepository customerRepository,
            IAddressRepository addressRepository,
            CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetBy(customerId, cancellationToken);
        if (customer is null) return TypedResults.NotFound();

        var existing = await addressRepository.GetByCustomerId(customerId, cancellationToken);
        var isCreate = existing is null;

        var entity = existing ?? ToEntity(Guid.NewGuid(), customerId, request);
        if (!isCreate)
            ApplyToEntity(entity, request);

        var saved = (await addressRepository.Upsert(entity, cancellationToken))!;

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

        var response = ToResponse(saved);
        if (isCreate)
            return TypedResults.Created($"{httpContext.Request.Path}", response);

        return TypedResults.Ok(response);
    }

    private static Address ToEntity(Guid addressId, Guid customerId, AdminPutAddressRequest request)
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

    private static void ApplyToEntity(Address entity, AdminPutAddressRequest request)
    {
        entity.Country = request.Country;
        entity.City = request.City;
        entity.ZipCode = request.ZipCode;
        entity.AddressLine1 = request.AddressLine1;
        entity.AddressLine2 = request.AddressLine2;
        entity.ContactPhone = request.ContactPhone;
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