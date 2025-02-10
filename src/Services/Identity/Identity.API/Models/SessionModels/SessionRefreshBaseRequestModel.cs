using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models.SessionModels;

public class SessionRefreshBaseRequestModel
{
    [Required]
    public string? RefreshToken { get; set; }

    public string? TokenType { get; set; } = "Bearer";
}