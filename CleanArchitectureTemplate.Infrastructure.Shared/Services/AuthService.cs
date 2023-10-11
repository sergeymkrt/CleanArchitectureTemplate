using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using CleanArchitectureTemplate.Domain.Services.External;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitectureTemplate.Infrastructure.Shared.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(string email, long id, string firstName, string lastName, string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, email),
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Name, $"{firstName} {lastName}"),
            new(ClaimTypes.GivenName, firstName),
            new(ClaimTypes.Surname, lastName)
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var securityKey = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP521));
        securityKey.ECDsa.ImportFromPem(_configuration["JWT:PrivateKey"]);
        
        var accessTokenHours = int.Parse(_configuration["JWT:AccessTokenValidityInHours"]);
        
        var token = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(accessTokenHours),
            claims: claims,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha512)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        throw new NotImplementedException();
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        throw new NotImplementedException();
    }
}
