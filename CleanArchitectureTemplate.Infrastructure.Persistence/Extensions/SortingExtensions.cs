using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using CleanArchitectureTemplate.Application.Exceptions;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;

public class SortingColumn
{
    [JsonPropertyName("orderBy")]
    public string Name { get; set; }
    [JsonPropertyName("dir")]
    public SortDirection Direction { get; set; } = SortDirection.Ascending;

    public bool IsDescending => Direction == SortDirection.Descending;

    public SortingColumn() { }

    public SortingColumn(string columnName, SortDirection direction = SortDirection.Ascending)
    {
        Name = columnName;
        Direction = direction;
    }
}

public static class SortingExtensions
{
    #region Sort
    public static IOrderedQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> data, SortingColumn sortingParam)
        where TEntity : class
    {
        return data.Sort(new List<SortingColumn> { sortingParam });
    }

    public static IOrderedQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> data, IEnumerable<SortingColumn> sortingParams)
        where TEntity : class
    {
        return data.Sort(sortingParams.ToDictionary());
    }

    /// <summary>
    /// Sorts the specified object with sort parameters.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="data">object To sort.</param>
    /// <param name="sortingParams">The sort options.</param>
    /// <returns>IOrderedEnumerable of sorted object</returns>
    /// <remarks></remarks>
    public static IOrderedQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> data, Dictionary<string, SortDirection> sortingParams)
        where TEntity : class
    {
        (IOrderedQueryable<TEntity> SortedData, System.Linq.Expressions.MethodCallExpression MethodCallExpression)? result = null;

        foreach (var sortingParam in sortingParams.Where(x => !string.IsNullOrEmpty(x.Key)))
        {
            //var column = typeof(TEntity).GetProperty(sortingParam.Key, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            //if (column != null)
            //{
            result = result == null ? data.OrderByRuntime(sortingParam.Key, sortingParam.Value)
                : data.ThenByRuntime(sortingParam.Key, sortingParam.Value, result.Value.MethodCallExpression);
            //}
        }

        return result.HasValue ? result.Value.SortedData ?? data.OrderBy(x => 0) : data.OrderBy(x => 0);
    }
    #endregion

    #region  OrderBy / ThenBy / ApplyOrder
    public static (IOrderedQueryable<TEntity> OrderedQueryable, MethodCallExpression MethodCallExpression) OrderByRuntime<TEntity>(this IQueryable<TEntity> source, SortingColumn sortingColumn)
        where TEntity : class
    {
        return source.OrderByRuntime(sortingColumn.Name, sortingColumn.IsDescending);
    }

    public static (IOrderedQueryable<TEntity> OrderedQueryable, MethodCallExpression MethodCallExpression) OrderByRuntime<TEntity>(this IQueryable<TEntity> source, string property, SortDirection direction)
        where TEntity : class
    {
        return source.OrderByRuntime(property, direction == SortDirection.Descending);
    }

    public static (IOrderedQueryable<TEntity> OrderedQueryable, MethodCallExpression MethodCallExpression) OrderByRuntime<TEntity>(this IQueryable<TEntity> source, string property, bool desc)
        where TEntity : class
    {
        string command = desc ? "OrderByDescending" : "OrderBy";
        return source.ApplyOrder(property, command);
    }


    public static (IOrderedQueryable<TEntity> OrderedQueryable, MethodCallExpression MethodCallExpression) ThenByRuntime<TEntity>(this IQueryable<TEntity> source, SortingColumn sortingColumn, MethodCallExpression methodCallExpression)
        where TEntity : class
    {
        return source.ThenByRuntime(sortingColumn.Name, sortingColumn.IsDescending, methodCallExpression);
    }

    public static (IOrderedQueryable<TEntity> OrderedQueryable, MethodCallExpression MethodCallExpression) ThenByRuntime<TEntity>(this IQueryable<TEntity> source, string property, SortDirection direction, MethodCallExpression methodCallExpression)
        where TEntity : class
    {
        return source.ThenByRuntime(property, direction == SortDirection.Descending, methodCallExpression);
    }

    public static (IOrderedQueryable<TEntity> OrderedQueryable, MethodCallExpression MethodCallExpression) ThenByRuntime<TEntity>(this IQueryable<TEntity> source, string property, bool desc, MethodCallExpression methodCallExpression)
        where TEntity : class
    {
        string command = desc ? "ThenByDescending" : "ThenBy";
        return source.ApplyOrder(property, command, methodCallExpression);
    }



    /// <summary>
    /// Applies the sort order.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static (IOrderedQueryable<TEntity> OrderedQueryable, MethodCallExpression MethodCallExpression) ApplyOrder<TEntity>(this IQueryable<TEntity> source, string propertyName, string methodName, MethodCallExpression methodCallExpression = null)
    {
        Type type = typeof(TEntity);
        var propertyHierarchy = propertyName.Split('.').ToList();

        ParameterExpression parameter = Expression.Parameter(type, "p");
        Expression body = parameter;

        //Expression accessParameter = propertyHierarchy.Aggregate(body, Expression.PropertyOrField);
        Expression accessParameter = parameter;
        foreach (var member in propertyHierarchy)
        {
            var property = accessParameter.Type.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (property == null)
            {
                throw new InvalidSortPropertyException(propertyName, SortExceptionCode.InvalidSortingProperty);
            }
            if (Attribute.IsDefined(property, typeof(NotMappedAttribute)) /*|| property.GetSetMethod() == null*/)
            {
                throw new InvalidSortPropertyException(propertyName, SortExceptionCode.NonSortingProperty);
            }
            accessParameter = Expression.PropertyOrField(accessParameter, member);
        }

        LambdaExpression orderByExpression = Expression.Lambda(accessParameter, parameter);
        MethodCallExpression resultExpression = Expression.Call(typeof(Queryable), methodName, new[] { type, accessParameter.Type }, methodCallExpression ?? source.Expression, Expression.Quote(orderByExpression));
        return ((IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExpression), resultExpression);
    }
    #endregion

    #region Private methods
    private static Dictionary<string, SortDirection> ToDictionary(this IEnumerable<SortingColumn> sortingParams)
    {
        var dictionary = new Dictionary<string, SortDirection>();
        foreach (var item in sortingParams)
        {
            dictionary.TryAdd(item.Name, item.Direction);
        }
        return dictionary;
    }

    private static IEnumerable<SortingColumn> ToEnumerable(this Dictionary<string, SortDirection> sortingParams)
    {
        return sortingParams.Select(x => new SortingColumn(x.Key, x.Value));
    }
    #endregion
}
