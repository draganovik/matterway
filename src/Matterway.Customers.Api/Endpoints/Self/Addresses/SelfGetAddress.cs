using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Endpoints.Self.Addresses;

public class SelfGetAddress : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "address", Handler)
            .WithName("SelfGetAddress").WithSummary("[self] Get own Address.")
            .WithTags(nameof(Address))
            .Produces<AddressResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User) ||
                RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AddressResponse>, NotFound, ForbidHttpResult>> Handler(
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