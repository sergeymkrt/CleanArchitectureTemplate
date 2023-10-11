using System.Security.Claims;

namespace CleanArchitectureTemplate.Domain.Services.External;

public interface IAuthService
{  
    string GenerateJwtToken(string email, long id, string firstName, string lastName, string[] roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}
