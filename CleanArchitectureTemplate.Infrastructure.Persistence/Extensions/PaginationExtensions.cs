using CleanArchitectureTemplate.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;

public static class PaginationExtensions
{
    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
    {
        var totalRecords = await queryable.CountAsync();

        var data = await queryable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>(pageNumber, pageSize, totalRecords, data);
    }
}
