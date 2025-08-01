using BusinessManagementApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusinessManagementApi.Helpers
{
    /// <summary>
    /// This class creates JWT tokens for authenticated users.
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Gets JWT-related settings from appsettings.json.
        /// </summary>
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token for a valid user.
        /// </summary>
        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim("UserId", user.Id),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),        
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName), 
                new Claim(ClaimTypes.Name, user.UserName),       
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generate refresh token 
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

    }
}