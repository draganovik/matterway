using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Matterway.Common.Pagination;
using Matterway.Customers.Api.Features.Customers.Contracts;
using Matterway.Customers.Api.Features.Customers.Data;
using Matterway.Customers.Api.Features.Customers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Customers.Endpoints;

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
        var baseUri = ApiResourceUriBuilder.BuildAbsoluteUri(httpContext, "Customers");

        var paginationResponse = new PaginationResponse<CustomerBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<CustomerBaseResponse>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Customer> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}