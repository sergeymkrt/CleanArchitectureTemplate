using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CleanArchitectureTemplate.Application.DTOs;
using CleanArchitectureTemplate.Application.Models;
using CleanArchitectureTemplate.Domain.Extensions;

namespace CleanArchitectureTemplate.Application.Extensions;

public static class DecimalExtensions
{
    public static bool IsCorrectDecimal(this decimal? value, int precision, int scale)
        => value == null || CheckingResult(value.Value, precision, scale);

    public static bool IsCorrectDecimal(this decimal value, int precision, int scale) => CheckingResult(value, precision, scale);

    public static bool CheckingResult(decimal value, int precision, int scale)
    {
        if (precision <= scale)
            return false;

        var integerPart = precision - scale;
        var regex = new Regex(@"^\-?\d{0," + integerPart + @"}(.\d{0," + scale + "})?$");
        return regex.IsMatch(value.ToString(CultureInfo.InvariantCulture));
    }

    public static string ToString(this decimal? value, string format)
    {
        return value.HasValue ? value.Value.ToString(format) : string.Empty;
    }

    public static string ToCsvString(this decimal? value, string format)
    {
        return value.HasValue ? $"\r\t{value.Value.ToString(format)}" : string.Empty;
    }
}

public static class DateTimeExtensions
{
    public static string ToString(this DateTime? value, string format)
    {
        return value.HasValue ? value.Value.ToString(format) : string.Empty;
    }

    public static int GetMonthDifferenceTo(this DateTime startDate, DateTime endDate)
    {
        return (endDate.Month - startDate.Month) + (endDate.Year - startDate.Year) * 12;
    }

    public static int GetYearDifferenceFrom(this DateTime endDate, DateTime startDate)
    {
        return (endDate.Year - startDate.Year);
    }

    public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
    {
        return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
    }

    public static bool LessThan(this DateTime t1, DateTime? t2)
    {
        return !t2.HasValue || t1 < t2.Value;
    }

    public static bool LessThanOrEqual(this DateTime t1, DateTime? t2)
    {
        return !t2.HasValue || t1 <= t2.Value;
    }

    public static bool IsThePreviousDayOf(this DateTime t1, DateTime? t2)
    {
        if (!t2.HasValue)
        {
            return false;
        }

        return t1.Equals(t2.Value.AddDays(-1));
    }
}

public static class StringExtensions
{
    public static void CheckAndAppend(this StringBuilder stringBuilder, string line)
    {
        if (!string.IsNullOrEmpty(line))
            stringBuilder.AppendLineIfNeeded(line);
    }

    public static void AppendLineIfNeeded(this StringBuilder stringBuilder, string line)
    {
        if (!string.IsNullOrEmpty(stringBuilder.ToString()))
            stringBuilder.Append($"\n{line}");
        else
        {
            stringBuilder.Append(line);
        }
    }

    public static string GetHostName(this string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri.Host : default;
    }

    public static DateTime GetLatestDateForQuarter(this string quarter)
    {
        var regex = new Regex("^Q[1-4][|][0-9]{4}$");
        if (regex.IsMatch(quarter))
        {
            quarter = quarter.TrimStart('Q');
            var dateParts = quarter.Split('|')
                .Select(x => Convert.ToInt32(x)).ToArray();

            return new DateTime(dateParts[1], dateParts[0] * 3, DateTime.DaysInMonth(dateParts[1], dateParts[0] * 3));
        }

        throw new ArgumentException($"No matched quarter {quarter} value.");
    }
}

public static class DictionaryExtensions
{
    public static MultiKeyDictionary<TKey1, TKey2, IEnumerable<TValue>> ToMultiKeyDictionary<TKey1, TKey2, TValue>(
        this IEnumerable<MultiKeyValuesDto<TKey1, TKey2, TValue>> multiKeyValues)
        where TKey1 : struct, IConvertible
        where TKey2 : struct, IConvertible
    {
        var multiKeyDictionary = new MultiKeyDictionary<TKey1, TKey2, IEnumerable<TValue>>();
        foreach (var item in multiKeyValues)
        {
            multiKeyDictionary.Add(item.Key1, item.Key2 ?? default, default, item.Values);
        }

        return multiKeyDictionary;
    }

    public static MultiKeyDictionary<TKey1, TKey2, TKey3, IEnumerable<TValue>> ToMultiKeyDictionary<TKey1, TKey2, TKey3, TValue>(
        this IEnumerable<MultiKeyValuesDto<TKey1, TKey2, TKey3, TValue>> multiKeyValues)
        where TKey1 : struct, IConvertible
        where TKey2 : struct, IConvertible
        where TKey3 : struct, IConvertible
    {
        var multiKeyDictionary = new MultiKeyDictionary<TKey1, TKey2, TKey3, IEnumerable<TValue>>();
        foreach (var item in multiKeyValues)
        {
            multiKeyDictionary.Add(item.Key1, item.Key2 ?? default, item.Key3 ?? default, item.Values);
        }

        return multiKeyDictionary;
    }

    public static MultiKeyDictionary<TKey1, TKey2, IEnumerable<TValue>> ToMultiEnumKeyDictionary<TKey1, TKey2, TValue>(
        this IEnumerable<MultiKeyValuesDto<int, int, TValue>> multiKeyValues)
        where TKey1 : struct, IConvertible
        where TKey2 : struct, IConvertible
    {
        var multiKeyDictionary = new MultiKeyDictionary<TKey1, TKey2, IEnumerable<TValue>>();
        foreach (var item in multiKeyValues)
        {
            multiKeyDictionary.Add(item.Key1.ToEnum<TKey1>(), (item.Key2 ?? default).ToEnum<TKey2>(), default, item.Values);
        }

        return multiKeyDictionary;
    }

    public static MultiKeyDictionary<TKey1, TKey2, TKey3, IEnumerable<TValue>> ToMultiEnumKeyDictionary<TKey1, TKey2, TKey3, TValue>(
        this IEnumerable<MultiKeyValuesDto<int, int, int, TValue>> multiKeyValues)
        where TKey1 : struct, IConvertible
        where TKey2 : struct, IConvertible
        where TKey3 : struct, IConvertible
    {
        var multiKeyDictionary = new MultiKeyDictionary<TKey1, TKey2, TKey3, IEnumerable<TValue>>();
        foreach (var item in multiKeyValues)
        {
            multiKeyDictionary.Add(item.Key1.ToEnum<TKey1>(), (item.Key2 ?? default).ToEnum<TKey2>(), (item.Key3 ?? default).ToEnum<TKey3>(), item.Values);
        }

        return multiKeyDictionary;
    }

    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        => dictionary.ContainsKey(key) ? dictionary[key] : default;

    public static void TryAddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TValue> values, Func<TValue, TKey> keySelector)
    {
        foreach (var item in values)
        {
            var key = keySelector(item);
            dictionary.TryAdd(key, item);
        }
    }
}
