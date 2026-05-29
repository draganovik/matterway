using System.ComponentModel.DataAnnotations;

namespace Matterway.Identity.Api.Features.System.Users.Contracts;

public class SystemCreateCustomerUserResponse
{
    public Guid Id { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
