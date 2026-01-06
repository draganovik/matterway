using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Customers;

public class DeleteCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Customers/{id:guid}", Handler)
            .WithName("DeleteCustomer").WithSummary("Delete Customer by id.")
            .WithTags(nameof(Customer))
            .Produces<DeleteCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteCustomerResponse>, NotFound>> Handler(
        Guid id,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var isDeleted = await customerRepository.Delete(id, cancellationToken);
        if (!isDeleted) TypedResults.NotFound();

        return TypedResults.Ok(new DeleteCustomerResponse
        {
            Id = id,
            Message = "Customer removed successfully."
        });
    }

    public record DeleteCustomerResponse
    {
        public Guid Id { get; init; }
        public string Message { get; init; } = "Customer removed successfully.";
    }
}