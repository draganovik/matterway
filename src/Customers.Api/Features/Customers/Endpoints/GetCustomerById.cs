using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Customers.Api.Features.Customers.Contracts;
using Customers.Api.Features.Customers.Data;
using Customers.Api.Features.Customers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Customers.Api.Features.Customers.Endpoints;

public class GetCustomerById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/{id:guid}", Handler)
            .WithName("GetCustomerById").WithSummary("Get Customer by id.")
            .WithTags(nameof(Customer))
            .Produces<CustomerBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CustomerBaseResponse>, NotFound>> Handler(
        Guid id,
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        return await customerRepository.GetById(id)
            is Customer value
            ? TypedResults.Ok(mapper.Map<CustomerBaseResponse>(value))
            : TypedResults.NotFound();
    }
}