using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Identity.Api.Features.Sessions.Domain;
using Common.Infrastructure.Enums;

namespace Identity.Api.Features.SystemUsers.Domain;

public class SystemUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    [PasswordPropertyText]
    [Required(ErrorMessage = "Password is required.")]
    public string? PasswordHash { get; set; }

    public IEnumerable<Session>? Sessions { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SystemUserRole Role { get; set; } = SystemUserRole.Customer;
}