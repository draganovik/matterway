using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Customers;

public class QueryCustomers(ICustomerRepository customerRepository, LinkGenerator linkGenerator) :
    Endpoint<PaginationRequestParameters, PaginationResponse<QueryCustomers.CustomerResponse>>
{
    public override void Configure()
    {
        Get("/customers");
        Version(1);
        Options(options =>
        {
            options.WithName("QueryCustomers")
                .WithSummary("Query Customers.")
                .WithTags(nameof(Customer))
                .Produces<PaginationResponse<CustomerResponse>>()
                .Produces(StatusCodes.Status204NoContent)
                .ProducesValidationProblem()
                .RequireAuthorization(policy => policy.RequireRole(
                    nameof(ERequestClaimsRole.Admin),
                    nameof(ERequestClaimsRole.Manager)));
        });
    }

    public override async Task HandleAsync(PaginationRequestParameters request, CancellationToken cancellationToken)
    {
        var total = await customerRepository.Count(cancellationToken);
        var entities = await customerRepository.Query(request.Page, request.PageSize, cancellationToken);
        var baseUri = linkGenerator.GetUriByName(HttpContext, "QueryCustomers");

        var response = entities
            .Select(MapToResponse)
            .ToList();

        var paginationResponse = PaginationResponse<CustomerResponse>.Create(
            response,
            total,
            request.Page,
            request.PageSize,
            baseUri);

        if (!entities.Any())
        {
            await Send.NoContentAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(paginationResponse, cancellationToken);
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