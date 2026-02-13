using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Customers.Api.Features.Self.Addresses;

public class SelfPutAddress : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Self, "address", Handler)
            .WithName("SelfPutAddress").WithSummary("[self] Create or update own Address.")
            .WithTags(nameof(Address))
            .Produces<AddressResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User) ||
                RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Ok<AddressResponse>, BadRequest<ProblemDetails>, NotFound, ForbidHttpResult>>
        Handler(
            AddressRequest request,
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
            address = MapToEntity(Guid.NewGuid(), customer.Id, request);
        else
            ApplyUpdates(address, request);

        var saved = await addressRepository.Upsert(address, cancellationToken);
        if (saved is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot upsert address."
            });

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

        return TypedResults.Ok(MapToResponse(saved));
    }

    public record AddressRequest
    {
        [Required]
        public string Country { get; init; } = string.Empty;

        [Required]
        public string City { get; init; } = string.Empty;

        [Required]
        public string ZipCode { get; init; } = string.Empty;

        [Required]
        public string AddressLine1 { get; init; } = string.Empty;

        [Required]
        public string AddressLine2 { get; init; } = string.Empty;

        [Required]
        public string ContactPhone { get; init; } = string.Empty;
    }

    public record AddressResponse
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public string? Country { get; init; }

        [Required]
        public string? City { get; init; }

        [Required]
        public string? ZipCode { get; init; }

        [Required]
        public string? AddressLine1 { get; init; }

        [Required]
        public string? AddressLine2 { get; init; }

        [Required]
        public string? ContactPhone { get; init; }
    }

    private static Address MapToEntity(Guid addressId, Guid customerId, AddressRequest request)
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

    private static void ApplyUpdates(Address entity, AddressRequest request)
    {
        entity.Country = request.Country;
        entity.City = request.City;
        entity.ZipCode = request.ZipCode;
        entity.AddressLine1 = request.AddressLine1;
        entity.AddressLine2 = request.AddressLine2;
        entity.ContactPhone = request.ContactPhone;
    }

    private static AddressResponse MapToResponse(Address entity)
    {
        return new AddressResponse
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