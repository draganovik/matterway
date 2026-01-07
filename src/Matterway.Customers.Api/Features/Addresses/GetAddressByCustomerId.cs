using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Providers.Persistence.AddressEntity;
using Matterway.Customers.Api.Providers.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Addresses;

public class GetAddressByCustomerId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/{customerId:guid}/Address", Handler)
            .WithName("GetAddressByCustomerId").WithSummary("Get Address by customer id.")
            .WithTags(nameof(Address))
            .Produces<AddressResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<AddressResponse>, NotFound, ForbidHttpResult>> Handler(
        Guid customerId,
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        IAddressRepository addressRepository,
        CancellationToken cancellationToken)
    {
        if (!RequestIdentity.TryGet(httpContext.User, out var userContext) ||
            (userContext.Role == ERequestRole.Customer && customerId != userContext.SystemUserId))
            return TypedResults.Forbid();

        var customer = await customerRepository.GetBy(customerId, cancellationToken);
        if (customer?.DefaultAddressId is null)
            return TypedResults.NotFound();

        var address = await addressRepository.GetByCustomerId(customer.Id, cancellationToken);
        if (address is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(address));
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