using System.Linq.Expressions;

namespace CleanArchitectureTemplate.Domain.SeedWork;

public interface IQueryRepository<T> where T : RegularEntity
{
    Task<T> GetByIdAsync(
        long id,
        Expression<Func<T, object>> include = null);

    Task<List<T>> GetWhereAsync(
        Expression<Func<T, bool>> predicate = null,
        Expression<Func<T, object>> include = null);
}
