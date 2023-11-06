using Microsoft.IdentityModel.Tokens;
using MinimalApiLogin.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalApiLogin.Service
{
    public class TokenService
    {
        private IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(Usuario usuario)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("username", usuario.UserName),
                new Claim("id", usuario.Id)
                
            };
            SymmetricSecurityKey chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SDASDASDFDSFDSFSDFSD2132314312312312"));
            var signinCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
                (
                expires: DateTime.Now.AddMinutes(10),
                claims: claims,
                signingCredentials: signinCredentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
