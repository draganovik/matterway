using System.Text.Json.Serialization;

namespace Matterway.Identity.Api.Features.Admin.UserPerms.Contracts;

public class AdminPatchSystemUserPermResponse
{
    public string Service { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PermissionLevel Level { get; set; }
}