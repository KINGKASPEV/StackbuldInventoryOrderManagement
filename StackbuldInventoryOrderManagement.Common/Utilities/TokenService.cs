using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackbuldInventoryOrderManagement.Common.Config;
using StackbuldInventoryOrderManagement.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StackbuldInventoryOrderManagement.Common.Utilities
{
    public static class TokenService
    {
        public static string GenerateUserToken(
            ApplicationUser user,
            string role,
            IOptions<AuthSettings> options
        )
        {
            var jwtData = options.Value;

            var username = user.Email ?? string.Empty;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.HomePhone, user.PhoneNumber ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("username", username ?? string.Empty),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtData.SecretKey)
            );

            var expiry = user.UserType == UserType.Admin
                ? TimeSpan.FromHours(jwtData.TokenLifeTimeInHours)
                : TimeSpan.FromDays(jwtData.TokenLifeTimeDays);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow + expiry,
                Audience = jwtData.Audience,
                Issuer = jwtData.Issuer,
                SigningCredentials = new SigningCredentials(
                    signingKey,
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }
    }
}
