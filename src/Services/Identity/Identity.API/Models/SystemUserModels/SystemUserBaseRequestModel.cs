using Identity.API.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Identity.API.Models.SystemUserModels;

public class SystemUserBaseRequestModel
{

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    [PasswordPropertyText]
    [Required(ErrorMessage = "Password is required.")]
    public string? Password { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SystemUserRole Role { get; set; }
}
