using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using FluentValidation.Results;

namespace Matterway.Customers.Api.Features.Customers;

public class CreateCustomer(ICustomerRepository customerRepository) :
    Endpoint<CreateCustomer.CustomerRequest, CreateCustomer.CustomerResponse>
{
    public override void Configure()
    {
        Post("/customers");
        Version(1);

        Options(options =>
        {
            options.WithName("CreateCustomer")
                .WithSummary("Create a Customer.")
                .WithTags(nameof(Customer))
                .Produces<CustomerResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
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

        var newEntity = MapToEntity(request);

        if (userContext.Role == ERequestClaimsRole.Customer && newEntity.SystemUserId != userContext.SystemUserId)
        {
            await Send.ForbiddenAsync(cancellationToken);
            return;
        }

        newEntity.Id = newEntity.SystemUserId;

        var createdCustomer = await customerRepository.Create(newEntity, cancellationToken);
        if (createdCustomer is null)
        {
            var errors = new List<ValidationFailure>
            {
                new(string.Empty, "Cannot create entity")
            };
            await HttpContext.Response.SendErrorsAsync(errors, StatusCodes.Status400BadRequest, null,
                cancellationToken);
            return;
        }

        await Send.CreatedAtAsync<GetCustomerById>(
            new { id = createdCustomer.Id },
            MapToResponse(createdCustomer),
            cancellation: cancellationToken);
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

    private static Customer MapToEntity(CustomerRequest request)
    {
        return new Customer
        {
            SystemUserId = request.SystemUserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            DefaultAddressId = request.DefaultAddressId
        };
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