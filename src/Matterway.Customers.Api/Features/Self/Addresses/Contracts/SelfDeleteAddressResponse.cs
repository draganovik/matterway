namespace Matterway.Customers.Api.Features.Self.Addresses.Contracts;

public record SelfDeleteAddressResponse
{
    public Guid Id { get; init; }
    public string Message { get; init; } = "Address removed successfully.";
}
