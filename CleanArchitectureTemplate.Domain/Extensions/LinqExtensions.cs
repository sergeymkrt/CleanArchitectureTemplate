using CleanArchitectureTemplate.Domain.SeedWork;

namespace CleanArchitectureTemplate.Domain.Extensions;

public static class LinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        collection.ToList().ForEach(action);
    }

    public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            target.Add(item);
        }
    }

    public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        var count = items.Count();
        for (var i = count - 1; i >= 0; i--)
        {
            source.Remove(items.ElementAt(i));
        }
    }

    public static void RemoveRange<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        var itemsToRemove = collection.Where(predicate);
        collection.RemoveRange(itemsToRemove);
    }

    public static IList<T> Clone<T>(this ICollection<T> source) where T : ICloneable
    {
        return source.Select(item => (T)item.Clone()).ToList();
    }

    #region Creator / Modifier / Generator
    public static T SetAggregateData<T>(this T entity, long userId, DateTimeOffset? date = null) where T : ICreator, IModifier
    {
        entity.SetCreator(userId, date);
        entity.SetModifier(userId, date);
        return entity;
    }

    #region List
    public static void SetCreatorData<T>(this IEnumerable<T> entities, long userId, DateTimeOffset? date = null) where T : ICreator
    {
        entities.ForEach(x => x.SetCreator(userId, date));
    }

    public static void SetModifierData<T>(this IEnumerable<T> entities, long userId, DateTimeOffset? date = null) where T : IModifier
    {
        entities.ForEach(x => x.SetModifier(userId, date));
    }

    public static void SetAggregateData<T>(this IEnumerable<T> entities, long userId, DateTimeOffset? date = null) where T : ICreator, IModifier
    {
        entities.ForEach(x => x.SetCreator(userId, date));
        entities.ForEach(x => x.SetModifier(userId, date));
    }
    #endregion List

    #endregion  Creator / Modifier / Generator
}
