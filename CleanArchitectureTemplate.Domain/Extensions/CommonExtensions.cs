using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.SeedWork;

namespace CleanArchitectureTemplate.Domain.Extensions;

public static class CommonExtensions
{
    public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider source, bool inherit) where T : Attribute
    {
        var attrs = source.GetCustomAttributes(typeof(T), inherit);

        return (attrs != null) ? (T[])attrs : Enumerable.Empty<T>();
    }
    
    public static IList ToGenericList(this IEnumerable collection, Type propertyType)
    {
        var type = propertyType.GenericTypeArguments[0];
        var listType = typeof(List<>).MakeGenericType(type);
        var list = (IList)Activator.CreateInstance(listType);

        foreach (var item in collection)
        {
            list?.Add(item);
        }

        return list;
    }
    
    public static List<MemberInfo> GetMembersFromExpression<TSource, TCondition>(
        this Expression<Func<TSource, TCondition>> expression)
    {
        var members = new List<MemberInfo>();
        if (expression != null)
        {
            if (expression.Body is NewExpression key)
            {
                foreach (var member in key.Members)
                {
                    members.Add(member);
                }
            }
            else if (expression.Body is UnaryExpression unaryExpression)
            {
                if (unaryExpression.Operand is MemberExpression memberExpression)
                {
                    members.Add(memberExpression.Member);
                }
            }
            else if (expression.Body is MemberExpression memberExpression)
            {
                members.Add(memberExpression.Member);
            }
            else if (expression.Body is MemberInitExpression memberInitExpression)
            {
                foreach (var memberBinding in memberInitExpression.Bindings)
                {
                    members.Add(memberBinding.Member);
                }
            }
        }

        return members;
    }
    
    public static string GetEntityDisplayName<TSource>(this TSource source) where TSource : class
    {
        switch (source)
        {
            case LookupEntity lookupEntity:
                {
                    return lookupEntity?.Name;
                }
            case RegularEntity regularEntity:
                {
                    return regularEntity?.DisplayName;
                }
            default:
                return "";
        }
    }

    public static string GetDisplayName(this MemberInfo member)
    {
        if (Attribute.GetCustomAttribute(member, typeof(DisplayNameAttribute)) is DisplayNameAttribute attribute)
        {
            return attribute.DisplayName;
        }

        return member.Name.GenerateDisplayName();
    }

    public static string GenerateDisplayName(this string propertyName)
    {
        var regex = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
        return regex.Replace(propertyName, " ")
            .Replace(" Id", "", StringComparison.InvariantCulture)
            .Replace(" ID", "", StringComparison.InvariantCulture);
    }
    
    public static string Sanitize(this string value)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value.Trim()))
        {
            return "";
        }

        return value.Trim();
    }
    
    public static decimal RoundByRule(this decimal value, int precision, RoundingRule rule)
    {
        return rule switch
        {
            RoundingRule.MathematicalRounding => decimal.Round(value, precision, MidpointRounding.AwayFromZero),
            RoundingRule.RoundingDown => decimal.Round(value, precision, MidpointRounding.ToZero),
            RoundingRule.RoundingUp => decimal.Round(value, precision, MidpointRounding.ToPositiveInfinity),
            _ => 0,
        };
    }
    
    public static string RemoveTrailingZeros(this decimal value, bool showZero)
    {
        return Convert.ToString(value == 0 ? showZero ? "0" : string.Empty : value.ToString("#,##0.00########"));
    }
    
    public static T DeepClone<T>(this T source) where T : class
    {
        var output = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(output);
    }
    
    public static string StripInvalidFileCharacters(this string fileName)
    {
        return fileName.StripWhiteSpaces().Replace(",", "").Replace("(", "").Replace(")", "");
    }
    
    /// <summary>
    /// Remove white spaces from a string. If string is null return nothing.
    /// </summary>
    public static string StripWhiteSpaces(this string aString)
    {
        if (aString is object)
        {
            aString = aString.Replace(" ", "");
        }

        return aString.Trim();
    }
    
    /// <summary>
    /// Adds trailing zeros to the end of the number.
    /// If numberOfDecimalPlaces is smaller than the current value, no padding occurs and the original value
    /// is returned as a string with no padding.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="showZero"></param>
    /// <param name="numberOfDecimalPlaces"></param>
    /// <returns></returns>
    public static string PadTrailingZeros(this decimal value, bool showZero, int numberOfDecimalPlaces)
    {
        if (value == 0)
        {
            if (showZero)
            {
                if (numberOfDecimalPlaces == 0)
                {
                    // Only return 0 in this case, as there may be need to return 0.00
                    return "0";
                }
            }
            else
            {
                return string.Empty;
            }
        }

        int placesPastDecimal = value.NumberOfDecimalPlaces();
        return value.ToCommaSeparatedDecimals(numberOfDecimalPlaces < placesPastDecimal ? placesPastDecimal : numberOfDecimalPlaces);
    }
    
    public static string PadTrailingZeros(this decimal? value, bool showZero, int numberOfDecimalPlaces)
    {
        if (!value.HasValue)
        {
            return string.Empty;
        }

        return value.Value.PadTrailingZeros(showZero, numberOfDecimalPlaces);
    }

    public static int NumberOfDecimalPlaces(this decimal value)
    {
        string numberAsString = value.ToString("#,###.####################");
        int indexOfDecimalPoint = numberAsString.IndexOf(".");
        if (indexOfDecimalPoint == -1) // No decimal point in number
        {
            return 0;
        }
        else
        {
            return numberAsString.Substring(indexOfDecimalPoint + 1).Length;
        }
    }

    public static int NumberOfDecimalPlaces(this decimal? value)
    {
        return value.HasValue ? value.Value.NumberOfDecimalPlaces() : 0;
    }

    public static string ToCommaSeparatedDecimals(this decimal value, int numberOfDecimalPlaces)
    {
        var response = "{0:N" + numberOfDecimalPlaces + "}";
        return string.Format("" + response, value);
    }
    
    public static string AddPercentSign(this string value, bool withSpace = false)
    {
        return withSpace ? value + " %" : value + "%";
    }

    public static decimal ToDecimalOrZero(this decimal? value)
    {
        return value ?? 0;
    }
    
    public static object ToDBValue(this object value)
    {
        return value ?? DBNull.Value;
    }

    public static object FromDBValue(this object value)
    {
        return Convert.IsDBNull(value) ? null : value;
    }
}
