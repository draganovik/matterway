using Asp.Versioning;
using AutoMapper;
using Customers.API.Features.Customers.Contracts;
using Customers.API.Features.Customers.Data;
using Customers.API.Features.Customers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Iterfaces;

namespace Customers.API.Features.Customers.Endpoints;

public class VerifyCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/VerifyBy", Handler)
            .WithName("VerifyCustomer").WithSummary("Verify Customer by id or system user id.")
            .WithTags(nameof(Customer))
            .Produces<CustomerBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CustomerBaseResponse>, NotFound, BadRequest>> Handler(
        [FromQuery]
        Guid? customerId,
        [FromQuery]
        Guid? systemUserId,
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        if (customerId.HasValue)
        {
            var customer = await customerRepository.GetById(customerId.Value);
            return customer != null
                ? TypedResults.Ok(mapper.Map<CustomerBaseResponse>(customer))
                : TypedResults.NotFound();
        }

        if (systemUserId.HasValue)
        {
            var customer = await customerRepository.GetBySystemUserId(systemUserId.Value);
            return customer != null
                ? TypedResults.Ok(mapper.Map<CustomerBaseResponse>(customer))
                : TypedResults.NotFound();
        }

        return TypedResults.BadRequest();
    }
}