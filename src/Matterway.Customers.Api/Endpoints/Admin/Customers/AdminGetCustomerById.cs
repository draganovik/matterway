using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Endpoints.Admin.Customers;

public class AdminGetCustomerById : IEndpoint
{
    private const string RouteName = nameof(AdminGetCustomerById);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "customers/{customerId:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Get Customer by id")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<CustomerResponse>, NotFound>> Handler(
        Guid customerId,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var entity = await customerRepository.GetBy(customerId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(MapToResponse(entity));
    }

    public record CustomerResponse
    {
        [Required]
        public Guid SystemUserId { get; init; }

        [Required]
        public string? FirstName { get; init; }

        [Required]
        public string? LastName { get; init; }

        [Required]
        public DateOnly BirthDate { get; init; }

        public Guid? DefaultAddressId { get; init; }
    }

    private static CustomerResponse MapToResponse(Customer entity)
    {
        return new CustomerResponse
        {
            SystemUserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}