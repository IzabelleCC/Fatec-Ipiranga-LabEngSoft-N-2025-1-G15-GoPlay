using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoPlay_Core.Entities;
using GoPlay_Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GoPlay_Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _secretKey = GetSecretKey();
        }

        private string GetSecretKey()
        {
            var key = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            return string.IsNullOrEmpty(key) ? _configuration["JWT_SECRET_KEY"] : key;
        }

        public async Task<string> GenerateToken(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("LoginTimeStamp", DateTime.UtcNow.ToString("o"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var audience = _configuration["Jwt:Audience"];
            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("Jwt:Audience is not configured.");

            var issuer = _configuration["Jwt:Issuer"];
            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("Jwt:Issuer is not configured.");

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: signingCredentials
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public string? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Obter o ID do usuário a partir das reivindicações
                var userIdClaim = principal.FindFirst("Id");
                return userIdClaim?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
