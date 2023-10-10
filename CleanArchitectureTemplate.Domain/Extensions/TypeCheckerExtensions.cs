using System.Collections;
using CleanArchitectureTemplate.Domain.Attributes;

namespace CleanArchitectureTemplate.Domain.Extensions;

public static class TypeCheckerExtensions
{
    /// <summary>
    /// Checks if the type is simple (Primitive)
    /// </summary>
    /// <param name="type">The type we are checking.</param>
    /// <returns></returns>
    public static bool IsSimpleType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return IsSimpleType(type.GetGenericArguments()[0]);
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
    }

    /// <summary>
    /// Checks if the type is Enum
    /// </summary>
    /// <param name="type">The type we are checking.</param>
    /// <returns></returns>
    public static bool IsEnumType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return IsEnumType(type.GetGenericArguments()[0]);
        return type.IsEnum;
    }

    /// <summary>
    /// Checks if the type is nullable
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullableType(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Checks if the type is Collection
    /// </summary>
    /// <param name="type">The type we are checking.</param>
    /// <returns></returns>
    public static bool IsCollection(this Type type)
        => type.IsGenericType &&
           (type.GetInterface(nameof(IEnumerable)) != null ||
            type.GetInterface(nameof(ICollection)) != null ||
            type.GetInterface(nameof(IList)) != null);

    /// <summary>
    /// Checks if the type is a sub-class of a generic type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType">The base generic type</param>
    /// <returns></returns>
    public static bool IsSubclassOfRawGeneric(this Type type, Type genericType)
    {
        while (type != null && type != typeof(object))
        {
            var currentType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (currentType == genericType)
            {
                return true;
            }
            type = type.BaseType;
        }

        return false;
    }

    /// <summary>
    ///  Determine if a type implements an interface
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType">The interface type</param>
    /// <returns></returns>
    public static bool IsTypeImplementsAnInterface(this Type type, Type interfaceType)
    {
        return type.GetInterfaces().Contains(interfaceType);
    }

    /// <summary>
    ///  Determine if a type implements a generic interface
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType">The base generic type</param>
    /// <returns></returns>
    public static bool IsTypeImplementsAnGenericInterface(this Type type, Type genericType)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType);
    }


    /// <summary>
    /// Checks if the type should skip audit logging.
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public static bool IsLogAuditingEnabled(this Type entityType)
    {
        return !entityType.GetCustomAttributes(typeof(SkipAuditLogAttribute), true).Any();
    }

    /// <summary>
    /// Retrieves the collection type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetCollectionElementType(this Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        // first try the generic way
        // this is easy, just query the IEnumerable<T> interface for its generic parameter
        var genericType = typeof(IEnumerable<>);
        foreach (var bt in type.GetInterfaces())
            if (bt.IsGenericType && bt.GetGenericTypeDefinition() == genericType)
                return bt.GetGenericArguments()[0];

        // If it's a dictionary we always return DictionaryEntry
        if (typeof(IDictionary).IsAssignableFrom(type))
            return typeof(DictionaryEntry);

        // if it's a list we look for an Item property with an int index parameter
        // where the property type is anything but object
        if (typeof(IList).IsAssignableFrom(type))
        {
            foreach (var prop in type.GetProperties())
            {
                if ("Item" != prop.Name || typeof(object) == prop.PropertyType)
                    continue;

                var ipa = prop.GetIndexParameters();
                if (1 == ipa.Length && typeof(int) == ipa[0].ParameterType)
                    return prop.PropertyType;
            }
        }

        if (!typeof(ICollection).IsAssignableFrom(type))
            return typeof(IEnumerable).IsAssignableFrom(type) ? typeof(object) : null;

        // If it's a collection, we look for an Add() method whose parameter is
        // anything but object
        foreach (var methodInfo in type.GetMethods())
        {
            if ("Add" != methodInfo.Name) continue;

            var pa = methodInfo.GetParameters();
            if (1 == pa.Length && typeof(object) != pa[0].ParameterType)
                return pa[0].ParameterType;
        }
        return typeof(IEnumerable).IsAssignableFrom(type) ? typeof(object) : null;
    }
}

