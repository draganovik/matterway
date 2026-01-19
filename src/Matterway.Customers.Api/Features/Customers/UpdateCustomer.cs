using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Customers;

public class UpdateCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Customers/{systemUserId:guid}", Handler)
            .WithName("UpdateCustomer").WithSummary("Update Customer by id.")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CustomerResponse>, NotFound, ForbidHttpResult>> Handler(Guid systemUserId,
        CustomerRequest request,
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        if (!RequestIdentity.TryGet(httpContext.User, out var userContext)) return TypedResults.Forbid();

        if (userContext.Role == ERequestRole.Customer && request.SystemUserId != userContext.SystemUserId)
            return TypedResults.Forbid();

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