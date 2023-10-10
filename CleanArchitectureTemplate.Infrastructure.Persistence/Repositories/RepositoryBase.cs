using System.Linq.Expressions;
using CleanArchitectureTemplate.Domain.Models;
using CleanArchitectureTemplate.Domain.SeedWork;
using CleanArchitectureTemplate.Infrastructure.Persistence.Contexts;
using CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : struct, IComparable
{
    public IUnitOfWork UnitOfWork => Context;
    protected readonly ApplicationDbContext Context;

    protected RepositoryBase(ApplicationDbContext context)
    {
        Context = context;
    }

    public virtual async Task<TEntity> GetByIdAsync(
        TKey id,
        bool enableTracking = false)
        => await BuildQueryInternal(enableTracking).FirstOrDefaultAsync(q => q.Id.Equals(id));

    public virtual Task<PaginatedResult<TEntity>> GetPaginatedAsync(Expression<Func<TEntity, bool>> predicate = null,
        string search = null,
        List<(string ColumnName, bool isAsc)> orderBy = null,
        int depth = 2,
        int pageNumber = 1,
        int pageSize = 10, 
        IEnumerable<SearchExpression> extraSearchExpressions = default)
    {
        var query = GetQueryable(predicate, search, orderBy, depth, extraSearchExpressions);
        return query.ToPaginatedResultAsync(pageNumber, pageSize);
    }

    public virtual IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>> predicate = null,
        string search = null,
        List<(string ColumnName, bool isAsc)> orderBy = null,
        int depth = 2,
        IEnumerable<SearchExpression> extraSearchExpressions = default,
        bool enableTracking = false)
    {
        return BuildQueryInternal(enableTracking)
            .ApplyPredicate(predicate)
            .ApplySearch(search, depth, extraSearchExpressions)
            .ApplySorting(orderBy);
    }

    public virtual IQueryable<TEntity> GetAll(
        string search,
        Expression<Func<TEntity, bool>> predicate = null,
        int depth = 2,
        IEnumerable<SearchExpression> extraSearchExpressions = default,
        bool enableTracking = false)
    {
        return GetQueryable(
            predicate: predicate,
            search: search,
            depth: depth,
            extraSearchExpressions: extraSearchExpressions,
            enableTracking: enableTracking);
    }

    public virtual IQueryable<TEntity> GetAll(
        ICollection<string> columns,
        string search,
        IEnumerable<SearchExpression> extraSearchExpressions = default,
        bool enableTracking = false)
        => BuildQueryInternal(enableTracking).AsSingleQuery().Search(columns, search, extraSearchExpressions);

    public virtual async Task<List<TEntity>> GetAllAsync(
        string search,
        Expression<Func<TEntity, bool>> predicate = null,
        IEnumerable<SearchExpression> extraSearchExpressions = default,
        bool enableTracking = false)
    {
        return await GetAll(
                search: search,
                predicate: predicate,
                extraSearchExpressions: extraSearchExpressions,
                enableTracking: enableTracking)
            .ToListAsync();
    }

    public virtual IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>> expression)
        => Context.Set<TEntity>().Where(expression);

    public virtual async Task<List<TEntity>> GetWhereAsync(
        Expression<Func<TEntity, bool>> predicate)
    {
        return await GetQueryable(predicate: predicate).ToListAsync();
    }

    public virtual async Task AddAsync(TEntity entity)
        => await Context.Set<TEntity>().AddAsync(entity);

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        var addRange = entities as TEntity[] ?? entities.ToArray();
        await Context.Set<TEntity>().AddRangeAsync(addRange);
    }

    public virtual void Remove(TEntity entity)
        => Context.Set<TEntity>().Remove(entity);

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        var tEntities = entities as TEntity[] ?? entities.ToArray();
        Context.Set<TEntity>().RemoveRange(tEntities);
    }

    public virtual Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        if (predicate != null)
        {
            return Context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        return Context.Set<TEntity>().AnyAsync(cancellationToken);
    }

    public virtual Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        if (predicate != null)
        {
            return Context.Set<TEntity>().CountAsync(predicate, cancellationToken);
        }

        return Context.Set<TEntity>().CountAsync(cancellationToken);
    }

    public abstract IQueryable<TEntity> BuildQuery();




    #region private methods
    private IQueryable<TEntity> BuildQueryInternal(bool enableTracking = false)
    {
        var query = BuildQuery();

        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
    #endregion
}
