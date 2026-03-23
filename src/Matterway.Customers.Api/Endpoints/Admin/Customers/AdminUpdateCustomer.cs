using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Endpoints.Admin.Customers;

public class AdminUpdateCustomer : IEndpoint
{
    private const string RouteName = nameof(AdminUpdateCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "customers/{systemUserId:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Update Customer by id")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<CustomerResponse>, NotFound, ForbidHttpResult>> Handler(Guid systemUserId,
        CustomerRequest request,
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

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record CustomerRequest
    {
        [Required]
        public Guid SystemUserId { get; init; }

        [Required]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        public string LastName { get; init; } = string.Empty;

        [Required]
        public DateOnly BirthDate { get; init; }

        public Guid? DefaultAddressId { get; init; }
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