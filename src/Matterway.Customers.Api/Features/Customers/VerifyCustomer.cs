using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Customers;

public class VerifyCustomer(ICustomerRepository customerRepository) :
    Endpoint<VerifyCustomer.VerifyCustomerRequest, VerifyCustomer.CustomerResponse>
{
    public override void Configure()
    {
        Get("/customers/verifyby");
        Version(1);
        Options(options =>
        {
            options.WithName("VerifyCustomer")
                .WithSummary("Verify Customer by id or system user id.")
                .WithTags(nameof(Customer))
                .Produces<CustomerResponse>()
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest)
                .AllowAnonymous();
        });
    }

    public override async Task HandleAsync(VerifyCustomerRequest request, CancellationToken cancellationToken)
    {
        if (request.CustomerId.HasValue)
        {
            var customer = await customerRepository.GetBy(request.CustomerId.Value, cancellationToken);
            if (customer is null)
            {
                await Send.NotFoundAsync(cancellationToken);
                return;
            }

            await Send.OkAsync(MapToResponse(customer), cancellationToken);
            return;
        }

        if (request.SystemUserId.HasValue)
        {
            var customer = await customerRepository.GetBySuid(request.SystemUserId.Value, cancellationToken);
            if (customer is null)
            {
                await Send.NotFoundAsync(cancellationToken);
                return;
            }

            await Send.OkAsync(MapToResponse(customer), cancellationToken);
            return;
        }

        AddError("Either 'customerId' or 'systemUserId' must be provided.");
        ThrowIfAnyErrors();
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

    public record VerifyCustomerRequest
    {
        public Guid? CustomerId { get; init; }

        public Guid? SystemUserId { get; init; }
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