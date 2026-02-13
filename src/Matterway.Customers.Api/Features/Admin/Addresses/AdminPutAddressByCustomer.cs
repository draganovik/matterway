using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Customers.Api.Features.Admin.Addresses;

public class AdminPutAddressByCustomer : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Admin, "customers/{customerId:guid}/address", Handler)
            .WithName("AdminPutAddressByCustomer")
            .WithSummary("[admin] Create or replace Customer address by customer id")
            .WithTags(nameof(Address))
            .Produces<AddressResponse>(StatusCodes.Status200OK)
            .Produces<AddressResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<AddressResponse>, Created<AddressResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handler(
            Guid customerId,
            AddressRequest request,
            HttpContext httpContext,
            ICustomerRepository customerRepository,
            IAddressRepository addressRepository,
            CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetBy(customerId, cancellationToken);
        if (customer is null) return TypedResults.NotFound();

        var existing = await addressRepository.GetByCustomerId(customerId, cancellationToken);
        var isCreate = existing is null;

        var entity = existing ?? MapToEntity(Guid.NewGuid(), customerId, request);
        if (!isCreate)
            ApplyUpdates(entity, request);

        var saved = await addressRepository.Upsert(entity, cancellationToken);
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

        var response = MapToResponse(saved);
        if (isCreate)
            return TypedResults.Created($"{httpContext.Request.Path}", response);

        return TypedResults.Ok(response);
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
        public Guid CustomerId { get; init; }

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