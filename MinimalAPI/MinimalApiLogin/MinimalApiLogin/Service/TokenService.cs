using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MinimalApiLogin.Models;
using System.Data;
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
        public ResponseModel GenerateToken(IdentityUser usuario, IList<string> roles)
        {
            
            
            Claim[] claims = new Claim[]
            {
                new Claim("username", usuario.UserName),
                new Claim("id", usuario.Id),
                


            };
            
            claims = claims.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role))).ToArray();


            SymmetricSecurityKey chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SDASDASDFDSFDSFSDFSD2132314312312312"));
            var signinCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
                (
                expires: DateTime.Now.AddMinutes(10),
                claims: claims,
                signingCredentials: signinCredentials                
                );
            ResponseModel res = new ResponseModel() 
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Role = roles[0]
            };
            return res;
        }
    }
}
