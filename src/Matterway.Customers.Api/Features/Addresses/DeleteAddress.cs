using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Customers.Api.Features.Addresses;

public class DeleteAddress : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Customers/{customerId:guid}/Address", Handler)
            .WithName("DeleteAddress").WithSummary("Delete Address by customer id.")
            .WithTags(nameof(Address))
            .Produces<DeleteAddressResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Ok<DeleteAddressResponse>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(
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
        var addressId = address.Id;

        customer.DefaultAddressId = null;
        var updatedCustomer = await customerRepository.Update(customer, cancellationToken);
        if (updatedCustomer is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot update customer default address."
            });

        var deleted = await addressRepository.Delete(addressId, cancellationToken);
        if (!deleted) return TypedResults.NotFound();

        return TypedResults.Ok(new DeleteAddressResponse
        {
            Id = addressId,
            Message = "Address removed successfully."
        });
    }

    public record DeleteAddressResponse
    {
        public Guid Id { get; init; }
        public string Message { get; init; } = "Address removed successfully.";
    }
}