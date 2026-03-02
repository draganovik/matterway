using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Endpoints.Admin.Customers;

public class AdminQueryCustomers : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "customers", Handler)
            .WithName("AdminQueryCustomers").WithSummary("[admin] Query Customers")
            .WithTags(nameof(Customer))
            .Produces<PaginationResponse<CustomerResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<CustomerResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] PaginationRequestParameters pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerRepository customerRepository,
            CancellationToken cancellationToken)
    {
        var total = await customerRepository.Count(cancellationToken);
        var entities = await customerRepository.Query(pagingQuery.Page, pagingQuery.PageSize, cancellationToken);
        var baseUri = linkGenerator.GetUriByName(httpContext, "QueryCustomers");

        var response = entities
            .Select(MapToResponse)
            .ToList();

        var paginationResponse = PaginationResponse<CustomerResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        if (entities.Count == 0) TypedResults.NoContent();

        return TypedResults.Ok(paginationResponse);
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