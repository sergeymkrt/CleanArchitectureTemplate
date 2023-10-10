using System.Security.Claims;
using CleanArchitectureTemplate.Application.Interfaces.Services;
using CleanArchitectureTemplate.Domain.Services.External;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureTemplate.Infrastructure.Shared.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILookupService _lookupService;

    public IdentityService(
        IHttpContextAccessor httpContextAccessor,
        ILookupService lookupService)
    {
        _httpContextAccessor = httpContextAccessor;
        _lookupService = lookupService;
    }

    public long? Id => Convert.ToInt64(_httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
    public string FirstName => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
    public string LastName => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
    public string Email => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? "Unknown";
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string FullName => $"{LastName} {FirstName}";

    public long SystemUserId => 1;
}
