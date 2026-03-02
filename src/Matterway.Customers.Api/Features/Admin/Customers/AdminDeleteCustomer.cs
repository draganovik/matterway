using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Admin.Customers;

public class AdminDeleteCustomer : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "customers/{systemUserId:guid}", Handler)
            .WithName("AdminDeleteCustomer").WithSummary("[admin] Delete Customer by id")
            .WithTags(nameof(Customer))
            .Produces<DeleteCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteCustomerResponse>, NotFound>> Handler(
        Guid systemUserId,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var isDeleted = await customerRepository.Delete(systemUserId, cancellationToken);
        if (!isDeleted) TypedResults.NotFound();

        return TypedResults.Ok(new DeleteCustomerResponse
        {
            Id = systemUserId,
            Message = "Customer removed successfully."
        });
    }

    public record DeleteCustomerResponse
    {
        public Guid Id { get; init; }
        public string Message { get; init; } = "Customer removed successfully.";
    }
}