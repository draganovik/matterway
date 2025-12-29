using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Customers.Api.Features.Customers;

public class VerifyCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/VerifyBy", Handler)
            .WithName("VerifyCustomer").WithSummary("Verify Customer by id or system user id.")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CustomerResponse>, NotFound, BadRequest>> Handler(
        [FromQuery]
        Guid? customerId,
        [FromQuery]
        Guid? systemUserId,
        ICustomerRepository customerRepository)
    {
        if (customerId.HasValue)
        {
            var customer = await customerRepository.GetBy(customerId.Value);
            return customer != null
                ? TypedResults.Ok(MapToResponse(customer))
                : TypedResults.NotFound();
        }

        if (systemUserId.HasValue)
        {
            var customer = await customerRepository.GetBySuid(systemUserId.Value);
            return customer != null
                ? TypedResults.Ok(MapToResponse(customer))
                : TypedResults.NotFound();
        }

        return TypedResults.BadRequest();
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