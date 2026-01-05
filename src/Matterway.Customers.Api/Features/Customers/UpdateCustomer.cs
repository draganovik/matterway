using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Customers;

public class UpdateCustomer(ICustomerRepository customerRepository) :
    Endpoint<UpdateCustomer.CustomerRequest, UpdateCustomer.CustomerResponse>
{
    public override void Configure()
    {
        Patch("/customers/{id:guid}");
        Version(1);
        Options(options =>
        {
            options.WithName("UpdateCustomer")
                .WithSummary("Update Customer by id.")
                .WithTags(nameof(Customer))
                .Produces<CustomerResponse>()
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status403Forbidden)
                .RequireAuthorization();
        });
    }

    public override async Task HandleAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        if (!UserContext.TryGet(User, out var userContext))
        {
            await Send.ForbiddenAsync(cancellationToken);
            return;
        }

        if (userContext.Role == ERequestClaimsRole.Customer && request.SystemUserId != userContext.SystemUserId)
        {
            await Send.ForbiddenAsync(cancellationToken);
            return;
        }

        var id = Route<Guid>("id");
        var entity = await customerRepository.GetBy(id, cancellationToken);
        if (entity is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        entity.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.BirthDate,
            request.DefaultAddressId);

        var updated = await customerRepository.Update(entity, cancellationToken);
        if (updated is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(MapToResponse(updated), cancellationToken);
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