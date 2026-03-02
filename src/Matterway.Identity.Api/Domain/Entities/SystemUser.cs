using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Domain.Entities;

public class SystemUser : IdentityUser<Guid>
{
    public DateTime Created { get; set; } = DateTime.UtcNow;
}