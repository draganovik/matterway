using Asp.Versioning;
using AutoMapper;
using Customers.API.Features.Customers.Contracts;
using Customers.API.Features.Customers.Data;
using Customers.API.Features.Customers.Domain;
using Customers.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;
using Common.Infrastructure.ModelTemplates;

namespace Customers.API.Features.Customers.Endpoints;

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