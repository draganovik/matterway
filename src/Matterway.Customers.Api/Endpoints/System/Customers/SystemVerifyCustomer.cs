using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Endpoints.System.Customers;

public class SystemVerifyCustomer : IEndpoint
{
    private const string RouteName = nameof(SystemVerifyCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.System, "customers/{customerId:guid}/verify", Handler)
            .WithName(RouteName).WithSummary("[system] Verify Customer by id")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireSystemAccessKey()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<CustomerResponse>, NotFound, BadRequest>> Handler(
        Guid customerId,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetBySuid(customerId, cancellationToken);
        return customer != null
            ? TypedResults.Ok(MapToResponse(customer))
            : TypedResults.NotFound();
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