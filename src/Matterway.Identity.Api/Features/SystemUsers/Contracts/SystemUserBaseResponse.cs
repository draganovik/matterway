using Matterway.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Matterway.Identity.Api.Features.SystemUsers.Contracts;

public class SystemUserBaseResponse
{
    public Guid Id { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public DateTime Created { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SystemUserRole Role { get; set; }
}