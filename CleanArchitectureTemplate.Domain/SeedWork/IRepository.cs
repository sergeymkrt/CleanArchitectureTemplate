using System.Linq.Expressions;
using CleanArchitectureTemplate.Domain.Models;

namespace CleanArchitectureTemplate.Domain.SeedWork;

public interface IRepository<TEntity, in TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : struct, IComparable
{
    IUnitOfWork UnitOfWork { get; }

    Task<TEntity> GetByIdAsync(
        TKey id,
        bool enableTracking = false);

    Task<PaginatedResult<TEntity>> GetPaginatedAsync(
        Expression<Func<TEntity, bool>> predicate = null,
        string search = null,
        List<(string ColumnName, bool isAsc)> orderBy = null,
        int depth = 2,
        int pageNumber = 1,
        int pageSize = 10,
        IEnumerable<SearchExpression> extraSearchExpressions = default);

    IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>> predicate = null,
        string search = null,
        List<(string ColumnName, bool isAsc)> orderBy = null,
        int depth = 2,
        IEnumerable<SearchExpression> extraSearchExpressions = default,
        bool enableTracking = false);

    IQueryable<TEntity> GetAll(
        string search,
        Expression<Func<TEntity, bool>> predicate = null,
        int depth = 2,
        IEnumerable<SearchExpression> extraSearchExpressions = default,
        bool enableTracking = false);

    IQueryable<TEntity> GetAll(
        ICollection<string> columns,
        string search,
        IEnumerable<SearchExpression> extraSearchExpressions = default,
        bool enableTracking = false);

    Task<List<TEntity>> GetAllAsync(
        string search,
        Expression<Func<TEntity, bool>> predicate = null,
        IEnumerable<SearchExpression> extraSearchExpressions = default,
        bool enableTracking = false);

    IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>> expression);

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entities);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}

