namespace Identity.API.Models;

public class SessionGetResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
}
