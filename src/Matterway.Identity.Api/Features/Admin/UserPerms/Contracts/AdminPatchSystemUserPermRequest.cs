using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Matterway.Identity.Api.Domain;

namespace Matterway.Identity.Api.Features.Admin.UserPerms.Contracts;

public class AdminPatchSystemUserPermRequest
{
    [Required(ErrorMessage = "Service is required.")]
    public string? Service { get; set; }

    [Required(ErrorMessage = "Permission level is required.")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PermissionLevel? Level { get; set; }
}
