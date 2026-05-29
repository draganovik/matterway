using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Admin.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Admin.Customers.Endpoints;

public class AdminGetCustomerById : IEndpoint
{
    private const string RouteName = nameof(AdminGetCustomerById);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "customers/{customerId:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Get Customer by id")
            .WithTags(nameof(Customer))
            .Produces<AdminBaseCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminBaseCustomerResponse>, NotFound>> Handler(
        Guid customerId,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var entity = await customerRepository.GetBy(customerId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(ToResponse(entity));
    }

    private static AdminBaseCustomerResponse ToResponse(Customer entity)
    {
        return new AdminBaseCustomerResponse
        {
            SystemUserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}
