namespace CleanArchitectureTemplate.Domain.SeedWork;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(long userId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesWithAuditAsync(long userId, Guid scopeId, CancellationToken cancellationToken = default);
}

