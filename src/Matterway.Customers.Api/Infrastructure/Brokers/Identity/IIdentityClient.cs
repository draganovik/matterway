using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Infrastructure.Brokers.Identity;

public interface IIdentityClient
{
    Task<BrokerResponse<CreateCustomerUserResponse>> CreateCustomerUserAsync(
        CreateCustomerUserRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record CreateCustomerUserRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    [MinLength(6)]
    public required string Password { get; init; }
}

public sealed record CreateCustomerUserResponse
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
}