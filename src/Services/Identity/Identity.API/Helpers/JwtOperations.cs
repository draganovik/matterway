using Identity.API.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.API.Helpers;

public static class JwtOperations
{

    public static (string token, SecurityTokenDescriptor descriptor) Generate(SystemUser user, IConfiguration config, bool isRefresh = false)
    {
        // Generate a JWT for the user session
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found."));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            IssuedAt = DateTime.UtcNow,
            Expires = isRefresh ? DateTime.UtcNow.AddDays(12) : DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), tokenDescriptor);
    }
}
