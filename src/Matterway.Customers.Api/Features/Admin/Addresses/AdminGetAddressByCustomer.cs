using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.ServiceDefaults.Api;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Admin.Addresses;

public class AdminGetAddressByCustomer : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "customers/{customerId:guid}/address", Handler)
            .WithName("AdminGetAddressByCustomer").WithSummary("[admin] Get Customer address by customer id")
            .WithTags(nameof(Address))
            .Produces<AddressResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsObserver(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<AddressResponse>, NotFound>> Handler(
        Guid customerId,
        IAddressRepository addressRepository,
        CancellationToken cancellationToken)
    {
        var entity = await addressRepository.GetByCustomerId(customerId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(entity));
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