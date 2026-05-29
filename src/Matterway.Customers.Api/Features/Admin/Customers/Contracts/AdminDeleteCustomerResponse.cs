namespace Matterway.Customers.Api.Features.Admin.Customers.Contracts;

public record AdminDeleteCustomerResponse
{
    public Guid Id { get; init; }
    public string Message { get; init; } = "Customer removed successfully.";
}
