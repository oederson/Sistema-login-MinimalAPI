using APIMinima.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIMinima.Service;
public static class TokenService
{
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public static ResponseModel GenerateToken(IdentityUser usuario, IList<string> roles)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["SymmetricSecurityKey"]);
        var tokenHandler = new JwtSecurityTokenHandler();
        Claim[] claims = new Claim[]
        {
            new Claim("username", usuario.UserName),
            new Claim("id", usuario.Id)
        };
        claims = claims.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role))).ToArray();
        SymmetricSecurityKey chave = new SymmetricSecurityKey(key);
        var signinCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256Signature),
            Audience = _configuration["CorsOrigin"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        ResponseModel res = new ResponseModel()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Role = roles[0]
        };
        return res;
    }
}

