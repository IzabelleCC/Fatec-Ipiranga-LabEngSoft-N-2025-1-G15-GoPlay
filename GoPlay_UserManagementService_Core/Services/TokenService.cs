using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using GoPlay_UserManagementService_Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace GoPlay_UserManagementService_Core.Services
{
    public class TokenService
    {
        public async Task<string> GenerateToken(UserEntity user)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9ASHDA98H9ah8HUYHasds0IF98765432"));
            var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "GoPlay_UserManagementService",
                audience: "GoPlay_UserManagementService",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return await Task.FromResult(tokenString);
        }
    }
}
