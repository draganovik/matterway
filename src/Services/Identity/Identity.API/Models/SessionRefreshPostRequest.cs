namespace Identity.API.Models;

public class SessionRefreshPostRequest
{
    public string RefreshToken { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
