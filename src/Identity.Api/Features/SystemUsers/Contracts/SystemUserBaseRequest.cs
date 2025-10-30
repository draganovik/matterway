using Matterway.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Identity.Api.Features.SystemUsers.Contracts;

public class SystemUserBaseRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    [PasswordPropertyText]
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string? Password { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SystemUserRole? Role { get; set; }
}