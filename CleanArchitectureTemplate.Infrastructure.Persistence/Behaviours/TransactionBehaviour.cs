using CleanArchitectureTemplate.Application.UseCases;
using CleanArchitectureTemplate.Domain.Services.External;
using CleanArchitectureTemplate.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Behaviours;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand, IRequest<TResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;

    public TransactionBehaviour(
        ApplicationDbContext dbContext,
        IIdentityService identityService)
    {
        _dbContext = dbContext ?? throw new ArgumentException(nameof(ApplicationDbContext));
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request,  RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = default(TResponse);

        try
        {
            if (_dbContext.HasActiveTransaction)
            {
                return await next();
            }

            var strategy = _dbContext.Database.CreateExecutionStrategy();

            var userId = _identityService.Id.HasValue && _identityService.Id.Value != default
                ? _identityService.Id.Value
                : _identityService.SystemUserId;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.BeginTransactionAsync();
                response = await next();

                await _dbContext.CommitTransactionAsync(transaction, userId, cancellationToken);
            });

            return response;
        }
        catch
        {
            throw;
        }
    }
}

