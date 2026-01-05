using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Customers;

public class UpdateCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Customers/{id:guid}", Handler)
            .WithName("UpdateCustomer").WithSummary("Update Customer by id.")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CustomerResponse>, NotFound, ForbidHttpResult>> Handler(Guid id,
        CustomerRequest request,
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out ERequestClaimsRole userRole))
            return TypedResults.Forbid();

        if (userRole == ERequestClaimsRole.Customer && request.SystemUserId != systemUserId)
            return TypedResults.Forbid();

        var entity = await customerRepository.GetBy(id, cancellationToken);
        if (entity is null)
            return TypedResults.NotFound();

        entity.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.BirthDate,
            request.DefaultAddressId);

        var updated = await customerRepository.Update(entity, cancellationToken);
        return updated is not null
            ? TypedResults.Ok(MapToResponse(updated))
            : TypedResults.NotFound();
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