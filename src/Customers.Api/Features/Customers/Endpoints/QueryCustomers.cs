using Asp.Versioning;
using AutoMapper;
using Customers.Api.Features.Customers.Contracts;
using Customers.Api.Features.Customers.Data;
using Customers.Api.Features.Customers.Domain;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Pagination;
using Customers.Api.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Features.Customers.Endpoints;

public class QueryCustomers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers", Handler)
            .WithName("QueryCustomers").WithSummary("Query Customers.")
            .WithTags(nameof(Customer))
            .Produces<PaginationResponse<CustomerBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<CustomerBaseResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] PagingQueryParams pagingQuery,
            HttpContext httpContext,
            ICustomerRepository customerRepository,
            IMapper mapper)
    {
        var total = await customerRepository.GetTotalEntities();
        var entities = await customerRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "Customers");

        var paginationResponse = new PaginationResponse<CustomerBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<CustomerBaseResponse>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Customer> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}
