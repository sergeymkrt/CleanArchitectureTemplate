using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;

public static class DbContextExtensions
{
    public static IQueryable Set(this DbContext context, Type entityType)
    {
        // Get the generic type definition
        var method = typeof(DbContextExtensions).GetMethod(nameof(DbContextExtensions.SetContext), BindingFlags.NonPublic | BindingFlags.Static);

        // Build a method with the specific type argument you're interested in
        method = method.MakeGenericMethod(entityType);

        return method.Invoke(null, new object[] { context }) as IQueryable;
    }

    private static IQueryable<TEntity> SetContext<TEntity>(this DbContext context) where TEntity : class
    {
        return context.Set<TEntity>();
    }
}
