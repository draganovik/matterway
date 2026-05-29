namespace Matterway.Customers.Api.Features.Public.Registration.Contracts;

public record PublicRegisterCustomerResponse
{
    public Guid SystemUserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateOnly BirthDate { get; init; }
}
