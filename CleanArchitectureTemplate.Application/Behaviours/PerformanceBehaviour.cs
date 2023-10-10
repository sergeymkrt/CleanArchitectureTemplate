using System.Diagnostics;
using CleanArchitectureTemplate.Domain.Services.External;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.Application.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly IIdentityService _currentUserService;

    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        IIdentityService currentUserService)
    {
        _timer = new Stopwatch();

        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 1000)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.Id ?? 0;

            _logger.LogWarning("Application Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId, _currentUserService.FullName ?? "Unknown", request);
        }

        return response;
    }

}
