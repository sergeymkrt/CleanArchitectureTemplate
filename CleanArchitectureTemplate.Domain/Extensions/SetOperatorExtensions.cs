using System.Reflection;
using CleanArchitectureTemplate.Domain.Models;

namespace CleanArchitectureTemplate.Domain.Extensions;

public static class SetOperatorExtensions
{
    public static bool SetEquals<TSource, TKey>(
        this TSource first,
        TSource second,
        Func<TSource, TKey> keySelector)
        where TSource : class
    {
        var keyType = typeof(TKey);
        if (!keyType.IsClass || keyType.IsSimpleType())
        {
            var firstKey = keySelector(first) as object;
            var secondKey = keySelector(second) as object;
            return firstKey.Equals(secondKey);
        }
        else
        {
            // Dynamic Type
            var firstKey = new DynamicValueModel(keySelector(first));
            var secondKey = new DynamicValueModel(keySelector(second));
            return firstKey.Equals(secondKey);
        }
    }

    public static bool EqualsBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> comparer = null)
        where TSource : class
    {

        var keyType = typeof(TKey);
        if (!keyType.IsClass || keyType.IsSimpleType())
        {
            var firstKeys = new HashSet<TKey>(first.Select(keySelector), comparer);
            var secondKeys = new HashSet<TKey>(second.Select(keySelector), comparer);
            return firstKeys.SetEquals(secondKeys);
        }
        else
        {
            // Dynamic Type
            var firstKeys = new HashSet<DynamicValueModel>(first.Select(x => new DynamicValueModel(keySelector(x))));
            var secondKeys = new HashSet<DynamicValueModel>(second.Select(x => new DynamicValueModel(keySelector(x))));
            return firstKeys.SetEquals(secondKeys);
        }
    }

    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> comparer = null)
        where TSource : class
    {
        var keyType = typeof(TKey);
        if (!keyType.IsClass || keyType.IsSimpleType())
        {
            var keys = new HashSet<TKey>(second.Select(keySelector), comparer);
            foreach (var item in first)
            {
                var key = keySelector(item);
                if (keys.Contains(key, comparer))
                {
                    yield return item;
                    keys.Remove(key);
                }
            }
        }
        else
        {
            var keys = new HashSet<DynamicValueModel>(second.Select(x => new DynamicValueModel(keySelector(x))));
            foreach (var item in first)
            {
                var key = new DynamicValueModel(keySelector(item));
                if (keys.Contains(key))
                {
                    yield return item;
                    keys.Remove(key);
                }
            }
        }
    }

    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> comparer = null)
        where TSource : class
    {
        var keyType = typeof(TKey);
        if (!keyType.IsClass || keyType.IsSimpleType())
        {
            comparer ??= EqualityComparer<TKey>.Default;

            var keys = new HashSet<TKey>(second.Select(keySelector), comparer);
            foreach (var item in first)
            {
                var key = keySelector(item);
                if (!keys.Contains(key, comparer))
                {
                    yield return item;
                    keys.Remove(key);
                }
            }
        }
        else
        {
            var keys = new HashSet<DynamicValueModel>(second.Select(x => new DynamicValueModel(keySelector(x))));
            foreach (var item in first)
            {
                var key = new DynamicValueModel(keySelector(item));
                if (!keys.Contains(key))
                {
                    yield return item;
                    keys.Remove(key);
                }
            }
        }
    }

    public static IEnumerable<TSource> UnionBy<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> comparer = null)
        where TSource : class
    {
        foreach (var item in first)
        {
            yield return item;
        }

        var keyType = typeof(TKey);
        if (!keyType.IsClass || keyType.IsSimpleType())
        {
            var keys = new HashSet<TKey>(first.Select(keySelector), comparer);
            foreach (var item in second)
            {
                var key = keySelector(item);
                if (!keys.Contains(key, comparer))
                {
                    yield return item;
                    keys.Remove(key);
                }
            }
        }
        else
        {
            var keys = new HashSet<DynamicValueModel>(second.Select(x => new DynamicValueModel(keySelector(x))));
            foreach (var item in second)
            {
                var key = new DynamicValueModel(keySelector(item));
                if (!keys.Contains(key))
                {
                    yield return item;
                    keys.Remove(key);
                }
            }
        }
    }

    public static object GetKeyValue<TSource, TKey>(
        this TSource source,
        Func<TSource, TKey> keySelector)
    {
        var keyType = typeof(TKey);
        var key = keySelector(source);
        if (!keyType.IsClass || keyType.IsSimpleType())
        {
            return key;
        }

        return (new DynamicValueModel(key)).Values[0];
    }

}
