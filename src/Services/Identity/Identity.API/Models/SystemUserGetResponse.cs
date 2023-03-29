using Identity.API.Enums;

namespace Identity.API.Models;

public class SystemUserGetResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public string Role { get; set; } = SystemUserRole.Customer.ToString();
}
