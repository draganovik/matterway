using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Admin.Customers;

public class AdminDeleteCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/customers/{systemUserId:guid}", Handler)
            .WithName("AdminDeleteCustomer").WithSummary("Delete Customer by id (admin).")
            .WithTags(nameof(Customer))
            .Produces<DeleteCustomerResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteCustomerResponse>, NotFound>> Handler(
        Guid systemUserId,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        var isDeleted = await customerRepository.Delete(systemUserId, cancellationToken);
        if (!isDeleted) TypedResults.NotFound();

        return TypedResults.Ok(new DeleteCustomerResponse
        {
            Id = systemUserId,
            Message = "Customer removed successfully."
        });
    }

    public record DeleteCustomerResponse
    {
        public Guid Id { get; init; }
        public string Message { get; init; } = "Customer removed successfully.";
    }
}