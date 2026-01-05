using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Customers;

public class GetCustomerById(ICustomerRepository customerRepository) :
    EndpointWithoutRequest<GetCustomerById.CustomerResponse>
{
    public override void Configure()
    {
        Get("/customers/{id:guid}");
        Version(1);
        Options(options =>
        {
            options.WithName("GetCustomerById")
                .WithSummary("Get Customer by id.")
                .WithTags(nameof(Customer))
                .Produces<CustomerResponse>()
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy => policy.RequireRole(
                    nameof(ERequestClaimsRole.Admin),
                    nameof(ERequestClaimsRole.Manager)));
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");

        var customer = await customerRepository.GetBy(id, cancellationToken);
        if (customer is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(MapToResponse(customer), cancellationToken);
    }

    public record CustomerResponse
    {
        [Required]
        public Guid Id { get; init; }

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
            Id = entity.Id,
            SystemUserId = entity.SystemUserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}