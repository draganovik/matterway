using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.API.Features.SystemUsers.Domain;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Features.Sessions.Services;

public static class JwtOperations
{
    public static (string Token, SecurityTokenDescriptor Descriptor) Generate(
        SystemUser user,
        IConfiguration configuration,
        bool isRefresh = false)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found.");
        var key = Encoding.UTF8.GetBytes(signingKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.Now,
            Expires = isRefresh ? DateTime.Now.AddDays(12) : DateTime.Now.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), tokenDescriptor);
    }
}