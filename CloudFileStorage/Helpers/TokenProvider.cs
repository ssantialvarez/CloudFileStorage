using System.Security.Claims;
using System.Text;
using CloudFileStorage.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CloudFileStorage.Helpers
{
    public class TokenProvider(IConfiguration configuration, IHttpContextAccessor _contextAccessor)
    { 
        public string Create(User user)
        {
            var secretKey = configuration["JWT:SECRET"];
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("JWT secret not found in configuration");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Name, user.username),
                    new Claim(ClaimTypes.Role, user.role.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };
            var tokenHandler = new JsonWebTokenHandler();
            string token = tokenHandler.CreateToken(tokenDescriptor);
            return token;
        }
        public string GetUserIdFromToken()
        {
            var user = _contextAccessor.HttpContext?.User;
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            if (user == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User not authenticated");
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.
            var userId = user.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found in token");
            return userId;
        }
        
        public string GetRoleFromToken()
        {
            var user = _contextAccessor.HttpContext?.User;
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            if (user == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User not authenticated");
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
                throw new UnauthorizedAccessException("Role not found in token");
            return role;
        }
        
    }
}
