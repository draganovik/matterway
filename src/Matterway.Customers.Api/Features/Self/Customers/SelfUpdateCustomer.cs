using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.ServiceDefaults.Api;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Self.Customers;

public class SelfUpdateCustomer : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Self, "profile", Handler)
            .WithName("SelfUpdateCustomer").WithSummary("[self] Update own Customer profile.")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CustomerResponse>, NotFound, ForbidHttpResult>> Handler(
        CustomerRequest request,
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        if (!RequestIdentity.CanManageOwnedResource(httpContext.User, request.SystemUserId))
            return TypedResults.Forbid();

        var entity = await customerRepository.GetBy(request.SystemUserId, cancellationToken);
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