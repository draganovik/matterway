using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Customers;

public class QueryCustomers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers", Handler)
            .WithName("QueryCustomers").WithSummary("Query Customers.")
            .WithTags(nameof(Customer))
            .Produces<PaginationResponse<CustomerResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<CustomerResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] PaginationRequestParameters pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerRepository customerRepository)
    {
        var total = await customerRepository.Count();
        var entities = await customerRepository.Query(pagingQuery.Page, pagingQuery.PageSize);
        var baseUri = linkGenerator.GetUriByName(httpContext, "QueryCustomers", null);

        var response = entities
            .Select(MapToResponse)
            .ToList();

        var paginationResponse = PaginationResponse<CustomerResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        return entities.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
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