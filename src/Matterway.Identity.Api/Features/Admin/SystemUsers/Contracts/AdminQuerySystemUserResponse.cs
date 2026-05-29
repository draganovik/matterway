using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Matterway.Identity.Api.Domain;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers.Contracts;

public class AdminQuerySystemUserResponse
{
    public Guid Id { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public DateTime Created { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EIdentityRole Role { get; set; }
}
