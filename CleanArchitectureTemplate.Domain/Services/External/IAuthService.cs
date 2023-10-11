using System.Security.Claims;
using CleanArchitectureTemplate.Domain.Aggregates.Users;

namespace CleanArchitectureTemplate.Domain.Services.External;

public interface IAuthService
{  
    string GenerateJwtToken(User user, string[] roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}
