using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Admin.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Admin.Customers.Endpoints;

public class AdminDeleteCustomer : IEndpoint
{
    private const string RouteName = nameof(AdminDeleteCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "customers/{systemUserId:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Delete Customer by id")
            .WithTags(nameof(Customer))
            .Produces<AdminDeleteCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminDeleteCustomerResponse>, NotFound>> Handler(
        Guid systemUserId,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var isDeleted = await customerRepository.Delete(systemUserId, cancellationToken);
        if (!isDeleted) return TypedResults.NotFound();

        return TypedResults.Ok(new AdminDeleteCustomerResponse
        {
            Id = systemUserId,
            Message = "Customer removed successfully."
        });
    }
}
