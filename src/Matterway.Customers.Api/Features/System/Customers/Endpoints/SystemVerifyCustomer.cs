using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.System.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.System.Customers.Endpoints;

public class SystemVerifyCustomer : IEndpoint
{
    private const string RouteName = nameof(SystemVerifyCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.System, "customers/{customerId:guid}/verify", Handler)
            .WithName(RouteName).WithSummary("[system] Verify Customer by id")
            .WithTags(nameof(Customer))
            .Produces<SystemVerifyCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireSystemAccessKey()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<SystemVerifyCustomerResponse>, NotFound, BadRequest>> Handler(
        Guid customerId,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetBySuid(customerId, cancellationToken);
        return customer != null
            ? TypedResults.Ok(ToResponse(customer))
            : TypedResults.NotFound();
    }

    private static SystemVerifyCustomerResponse ToResponse(Customer entity)
    {
        return new SystemVerifyCustomerResponse
        {
            SystemUserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}
