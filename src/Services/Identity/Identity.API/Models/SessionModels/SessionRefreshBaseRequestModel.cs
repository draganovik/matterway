namespace Identity.API.Models.SessionModels;

public class SessionRefreshBaseRequestModel
{
    public string? RefreshToken { get; set; }
    public string? TokenType { get; set; } = "Bearer";
}
