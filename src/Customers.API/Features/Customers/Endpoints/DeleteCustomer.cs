using Asp.Versioning;
using Customers.Api.Features.Customers.Data;
using Customers.Api.Features.Customers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Customers.Api.Features.Customers.Endpoints;

public class DeleteCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Customers/{id:guid}", Handler)
            .WithName("DeleteCustomer").WithSummary("Delete Customer by id.")
            .WithTags(nameof(Customer))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(
        Guid id,
        ICustomerRepository customerRepository)
    {
        var isDeleted = await customerRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}