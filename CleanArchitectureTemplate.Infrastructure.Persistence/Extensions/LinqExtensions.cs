using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Extensions;
using CleanArchitectureTemplate.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Extensions;

public static class LinqExtensions
{
    /// <summary>
    /// Generates a filter for searching using All the public properties
    /// on the <typeparamref name="TSource"/>
    /// that are writable to DB if a search term is provided.
    /// Uses the <see cref="DbFunctions"/> from <see cref="EF"/> to perform a LIKE %search% query.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of <see cref="IQueryable"/> we are applying this filter.
    /// </typeparam>
    /// <param name="source">The instance passed from the extension, <see cref="IQueryable"/> item.</param>
    /// <param name="search">The search term.</param>
    /// <param name="extraSearchExpressions">
    /// Extra search expressions (esp.) for searching through collections.
    /// </param>
    /// <returns>The <see cref="IQueryable{TSource}"/> passed in, with or without modifications</returns>
    public static IQueryable<TSource> Search<TSource>(
        this IQueryable<TSource> source, string search,
        int depth = 2,
        IEnumerable<SearchExpression> extraSearchExpressions = default)
        => string.IsNullOrWhiteSpace(search)
            ? source
            : Search(source, Utilities.GetEntityColumns<TSource>(depth), search, extraSearchExpressions);

    /// <summary>
    /// Generates a filter for searching using the column names passed directly if a search term is provided.
    /// Uses the <see cref="DbFunctions"/> from <see cref="EF"/> to perform a LIKE %search% query.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of <see cref="IQueryable"/> we are applying this filter.
    /// </typeparam>
    /// <param name="source">The instance passed from the extension, <see cref="IQueryable"/> item.</param>
    /// <param name="columns">
    /// The column names we are filtering.
    /// Supports navigation properties such as `User.Firstname` searches using the dot (.) notation.
    /// </param>
    /// <param name="search">The search term.</param>
    /// <param name="extraSearchExpressions">
    /// Extra search expressions (esp.) for searching through collections.
    /// </param>
    /// <returns>The <see cref="IQueryable{TSource}"/> passed in, with or without modifications</returns>
    public static IQueryable<TSource> Search<TSource>(
        this IQueryable<TSource> source, ICollection<string> columns,
        string search, IEnumerable<SearchExpression> extraSearchExpressions = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(search) || columns.Count == 0) return source; // We can exit anyway

            Expression containsExpression = null;
            Expression searchTermExp = Expression.Constant($"%{search.Trim()}%", typeof(string));
            var searchInstance = Expression.Parameter(typeof(TSource), "searchTerm");

            var enumSelectors = new List<MemberExpression>();

            foreach (var col in columns)
            {
                try
                {
                    BuildColumnQueryExpression(searchInstance, col, enumSelectors, searchTermExp,
                        ref containsExpression);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            // Map with Enum query
            FilterEnums(enumSelectors, search, ref containsExpression);

            // Map with Collection queries
            if (extraSearchExpressions != null)
                BuildCollectionsQueryExpression(searchInstance, search, extraSearchExpressions, searchTermExp,
                    ref containsExpression);

            Expression completedExpression = null;
            if (containsExpression != null)
                completedExpression = Expression.Call(
                    typeof(Queryable), "Where", new[] { source.ElementType }, source.Expression,
                    Expression.Lambda<Func<TSource, bool>>(containsExpression, searchInstance));

            return completedExpression != null ? source.Provider.CreateQuery<TSource>(completedExpression) : source;
        }
        catch (Exception)
        {
            return source;
        }
    }

    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        List<(string ColumnName, bool isAsc)> orderBy = null) where T : class
    {
        if (orderBy == null || (orderBy.Count == 1 && string.IsNullOrWhiteSpace(orderBy.First().ColumnName)))
        {
            orderBy = new List<(string ColumnName, bool isAsc)>() { ("id", false) };
        }

        Func<IQueryable<T>, IOrderedQueryable<T>> orderByExpr = x =>
            x.Sort(orderBy.Select(s =>
                new SortingColumn(s.ColumnName, s.isAsc ? SortDirection.Ascending : SortDirection.Descending)));

        IQueryable<T> res = orderByExpr(query);
        return res;
    }

    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        string search,
        int depth = 2,
        IEnumerable<SearchExpression> extraSearchExpressions = default)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Search(search, depth, extraSearchExpressions);
        }

        return query;
    }
    
    public static IQueryable<T> ApplyPredicate<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> predicate)
    {
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query;
    }
    
    /// <summary>
    /// Builds the expression for each column we need to work with.
    /// </summary>
    /// <param name="searchInstance">
    /// The search instance used to instantiate local parameter
    /// </param>
    /// <param name="col">The column name we are searching.</param>
    /// <param name="enumSelectors">
    /// Enums that are used to build separate search for enum fields.
    /// </param>
    /// <param name="searchTermExp">
    /// The actual search term we are using to filter data
    /// </param>
    /// <param name="containsExpression">
    /// The expression built after building our query.
    /// </param>
    private static void BuildColumnQueryExpression(
        Expression searchInstance, string col,
        ICollection<MemberExpression> enumSelectors,
        Expression searchTermExp, ref Expression containsExpression)
    {
        MemberExpression selector = null;
        var instanceProperty = searchInstance;
        foreach (var field in col.Split('.'))
        {
            selector = Expression.PropertyOrField(instanceProperty, field);
            instanceProperty = selector;
        }

        Expression containableQuery = selector;
        if (selector != null && selector.Member is PropertyInfo propertyInfo && propertyInfo.PropertyType != typeof(string))
        {
            // Enum selectors are handled accordingly
            if (propertyInfo.PropertyType.IsEnumType())
            {
                enumSelectors.Add(selector);
                return;
            }

            if (!propertyInfo.PropertyType.IsSimpleType()) return;
            //Execute: selector.ToString()
            var methodToString = typeof(object).GetMethod("ToString", Type.EmptyTypes);
            if (methodToString != null) containableQuery = Expression.Call(selector, methodToString);
        }

        if (containableQuery == null) return;

        //Call the EF.Functions.Like(column, '%searchTerm%')
        var methodLike = typeof(Microsoft.EntityFrameworkCore.DbFunctionsExtensions)
            .GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) });
        if (methodLike == null) return;

        Expression containsQuery =
            Expression.Call(methodLike, Expression.Constant(EF.Functions), containableQuery, searchTermExp);
        containsExpression = containsExpression == null
            ? containsQuery
            : Expression.Or(containsExpression, containsQuery);
    }

    /// <summary>
    /// Filters enums that have been found in the query and returns the modified expression.
    /// </summary>
    /// <param name="enumSelectors">The list of enums found in the query.</param>
    /// <param name="searchTerm">The search term</param>
    /// <param name="containedExpression">The expression built before applying Enum filters</param>
    private static void FilterEnums(
        IEnumerable<MemberExpression> enumSelectors, string searchTerm,
        ref Expression containedExpression)
    {
        try
        {
            searchTerm = searchTerm.ToLower();
            foreach (var selector in enumSelectors)
            {
                try
                {
                    var propertyInfo = selector.Member as PropertyInfo;
                    if (propertyInfo == null) continue;

                    var enumsMatch = propertyInfo.PropertyType
                        .ToEnumItemList()
                        .Where(e => e.DisplayText.ToLower().Contains(searchTerm) || e.Name.ToLower().Contains(searchTerm))
                        .Select(e => e.RawValue)
                        .ToList();
                    if (!enumsMatch.Any()) continue;

                    var enumsMatchExpression = Expression.Constant(enumsMatch.UpdateListType(propertyInfo.PropertyType), typeof(List<>).MakeGenericType(propertyInfo.PropertyType));
                    var methodContains = typeof(List<>).MakeGenericType(propertyInfo.PropertyType)
                        .GetMethod("Contains", new[] { propertyInfo.PropertyType });
                    if (methodContains == null) continue;

                    Expression containsQuery = Expression.Call(enumsMatchExpression, methodContains, selector);
                    containedExpression = containedExpression == null
                        ? containsQuery
                        : Expression.Or(containedExpression, containsQuery);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// Builds the expression for each column we need to work with.
    /// </summary>
    /// <param name="searchInstance">
    /// The search instance used to instantiate local parameter
    /// </param>
    /// <param name="searchTerm">The string search term we are using</param>
    /// <param name="extraSearchExpressions">
    /// The extra search expressions we are building respective collections query.
    /// </param>
    /// <param name="searchTermExp">
    /// The actual search term we are using to filter data
    /// </param>
    /// <param name="containsExpression">
    /// The expression built after building our query.
    /// </param>
    private static void BuildCollectionsQueryExpression(
        Expression searchInstance, string searchTerm,
        IEnumerable<SearchExpression> extraSearchExpressions,
        Expression searchTermExp, ref Expression containsExpression)
    {

        //Call the EF.Functions.Like(column, '%searchTerm%')
        var methodLike = typeof(Microsoft.EntityFrameworkCore.DbFunctionsExtensions)
            .GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) });
        if (methodLike == null) return;

        foreach (var searchExpression in extraSearchExpressions)
        {
            MemberExpression selector = null, innerSelector = null;
            var instanceProperty = searchInstance;
            // Supporting first levels for now with Any joins
            if (searchExpression.Children == null || searchExpression.Join == null) continue;

            foreach (var field in searchExpression.Column.Split('.'))
            {
                selector = Expression.PropertyOrField(instanceProperty, field);
                instanceProperty = selector;
            }
            // We can't proceed without root prop
            if (selector == null) continue;
            var selectorMember = selector.Member as PropertyInfo;
            if (selectorMember == null || !selectorMember.PropertyType.IsCollection()) continue;
            // Build inner expression for any
            var selectorCollectionType = selectorMember.PropertyType.GetCollectionElementType();
            if (selectorCollectionType == null) continue;
            var collectionMethodName = searchExpression.Join.ToString() ?? "Any";
            // The parameter supplied to the any function
            var anyInputParameterExpression = Expression.Parameter(selectorCollectionType, $"{collectionMethodName}Parameter");
            Expression searchChildrenExpression = null;
            foreach (var expressionChild in searchExpression.Children)
            {
                Expression innerInstanceProperty = anyInputParameterExpression;
                foreach (var field in expressionChild.Column.Split('.'))
                {
                    innerSelector = Expression.PropertyOrField(innerInstanceProperty, field);
                    innerInstanceProperty = innerSelector;
                }
                if (innerSelector == null) continue; // We failed to build inner query

                // Check if the data type is string to proceed
                Expression anyQuery = innerSelector;
                if (innerSelector.Member is PropertyInfo innerPropertyInfo && innerPropertyInfo.PropertyType != typeof(string))
                {
                    if (!innerPropertyInfo.PropertyType.IsSimpleType() && !innerPropertyInfo.PropertyType.IsEnumType()) continue;
                    if (innerPropertyInfo.PropertyType.IsEnumType())
                    {
                        var enumsMatch = innerPropertyInfo.PropertyType
                            .ToEnumItemList()
                            .Where(e => e.DisplayText.ToLower().Contains(searchTerm) || e.Name.ToLower().Contains(searchTerm))
                            .Select(e => e.RawValue)
                            .ToList();
                        if (!enumsMatch.Any()) continue;

                        var enumsMatchExpression = Expression.Constant(
                            enumsMatch.UpdateListType(innerPropertyInfo.PropertyType),
                            typeof(List<>).MakeGenericType(innerPropertyInfo.PropertyType));
                        var methodContains = typeof(List<>).MakeGenericType(innerPropertyInfo.PropertyType)
                            .GetMethod("Contains", new[] { innerPropertyInfo.PropertyType });
                        if (methodContains == null) continue;

                        Expression containsQuery = Expression.Call(enumsMatchExpression, methodContains, innerSelector);
                        searchChildrenExpression = searchChildrenExpression == null
                            ? containsQuery
                            : Expression.Or(searchChildrenExpression, containsQuery);

                        // We are done with this step, continue
                        continue;
                    }

                    //Execute: innerSelector.ToString()
                    var methodToString = typeof(object).GetMethod("ToString", Type.EmptyTypes);
                    if (methodToString != null) anyQuery = Expression.Call(anyQuery, methodToString);
                }

                Expression likeQuery =
                    Expression.Call(methodLike, Expression.Constant(EF.Functions), anyQuery, searchTermExp);
                // This allows us to have ORs within the lambda expression
                searchChildrenExpression = searchChildrenExpression == null
                    ? likeQuery
                    : Expression.Or(searchChildrenExpression, likeQuery);
            }

            // Build the any expression now
            if (searchChildrenExpression == null) continue;

            Expression completedExpression = Expression.Call(
                typeof(Enumerable), collectionMethodName, new[] { selectorCollectionType },
                selector, Expression.Lambda(searchChildrenExpression, anyInputParameterExpression));

            containsExpression = containsExpression == null
                ? completedExpression
                : Expression.Or(containsExpression, completedExpression);
        }
    }

    private static IList UpdateListType(this IEnumerable<object> enums, Type type)
    {
        var listType = typeof(List<>).MakeGenericType(type);
        var typedList = (IList)Activator.CreateInstance(listType);
        if (typedList == null) throw new Exception();
        foreach (var @enum in enums)
            typedList.Add(@enum);
        return typedList;
    }

}
