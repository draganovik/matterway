using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Admin.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Admin.Customers.Endpoints;

public class AdminUpdateCustomer : IEndpoint
{
    private const string RouteName = nameof(AdminUpdateCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "customers/{systemUserId:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Update Customer by id")
            .WithTags(nameof(Customer))
            .Produces<AdminBaseCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminBaseCustomerResponse>, NotFound, ForbidHttpResult>> Handler(Guid systemUserId,
        AdminUpdateCustomerRequest request,
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var entity = await customerRepository.GetBy(systemUserId, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        entity.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.BirthDate,
            request.DefaultAddressId);

        var updated = await customerRepository.Update(entity, cancellationToken);
        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(updated));
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
