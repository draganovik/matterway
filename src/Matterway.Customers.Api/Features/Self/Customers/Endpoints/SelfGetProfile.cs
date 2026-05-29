using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Self.Customers.Endpoints;

public class SelfGetProfile : IEndpoint
{
    private const string RouteName = nameof(SelfGetProfile);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "profile", Handler)
            .WithName(RouteName).WithSummary("[self] Get own Customer profile.")
            .WithTags(nameof(Customer))
            .Produces<SelfBaseCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<SelfBaseCustomerResponse>, NotFound>> Handler(
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.NotFound();

        var entity = await customerRepository.GetBy(customerId.Value, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(entity));
    }

    private static SelfBaseCustomerResponse ToResponse(Customer entity)
    {
        return new SelfBaseCustomerResponse
        {
            SystemUserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}