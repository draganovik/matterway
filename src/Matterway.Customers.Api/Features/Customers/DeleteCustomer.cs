using FastEndpoints;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Customers;

public class DeleteCustomer(ICustomerRepository customerRepository) :
    EndpointWithoutRequest<DeleteCustomer.DeleteCustomerResponse>
{
    public override void Configure()
    {
        Delete("/customers/{id:guid}");
        Version(1);
        Options(options =>
        {
            options.WithName("DeleteCustomer")
                .WithSummary("Delete Customer by id.")
                .WithTags(nameof(Customer))
                .Produces<DeleteCustomerResponse>()
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy => policy.RequireRole(
                    nameof(ERequestClaimsRole.Admin),
                    nameof(ERequestClaimsRole.Manager)));
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        var isDeleted = await customerRepository.Delete(id, cancellationToken);
        if (!isDeleted)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(new DeleteCustomerResponse
        {
            Id = id,
            Message = "Customer removed successfully."
        }, cancellationToken);
    }

    public record DeleteCustomerResponse
    {
        public Guid Id { get; init; }
        public string Message { get; init; } = "Customer removed successfully.";
    }
}