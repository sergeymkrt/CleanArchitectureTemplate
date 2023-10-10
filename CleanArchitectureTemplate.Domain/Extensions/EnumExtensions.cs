using System.Reflection;
using CleanArchitectureTemplate.Domain.Attributes;
using CleanArchitectureTemplate.Domain.Exceptions;
using CleanArchitectureTemplate.Domain.Models;
using Microsoft.OpenApi.Extensions;

namespace CleanArchitectureTemplate.Domain.Extensions;

public static class EnumExtensions
{
    public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute: Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        return type.GetField(name)?.GetCustomAttribute<TAttribute>();
    }
    
    public static int GetEnumValue(this Enum enumValue)
    {
        try
        {
            return Convert.ToInt32(enumValue);
        }
        catch (Exception)
        {
            return BitConverter.ToInt32(new byte[] { 0, 0, 0, Convert.ToByte(enumValue) }, 0);
        }
    }
    
    public static IEnumerable<int> GetEnumItemsByAttribute<TAttribute>(this Type enumType)
        where TAttribute : Attribute
    {
        return enumType.GetEnumItemsByAttribute<TAttribute>(x => true);
    }
    
    public static IEnumerable<int> GetEnumItemsByAttribute<TAttribute>(this Type enumType, Func<TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        if (enumType.IsEnum)
        {
            return enumType.GetFields()
                .Where(f => f.GetAttributes<TAttribute>(false).Any(predicate))
                .Select(f => ((Enum)f.GetValue(f.Name)).GetEnumValue());
        }

        return null;
    }
    
    public static string GetShortName(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<EnumDisplayAttribute>();
        return attribute?.ShortName;
    }
    
    public static string GetDescription(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<EnumDisplayAttribute>();
        return attribute?.Description;
    }

    public static bool GetDefaultValue(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<EnumDisplayAttribute>();
        return attribute?.IsDefault ?? default;
    }

    public static bool GetActiveValue(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<EnumDisplayAttribute>();
        return attribute?.IsActive ?? default;
    }

    public static int GetGroupValue(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<EnumDisplayAttribute>();
        return attribute?.GroupId ?? default;
    }

    public static int GetOrderValue(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<EnumDisplayAttribute>();
        return attribute?.Order ?? default;
    }

    public static int GetTypeValue(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<EnumDisplayAttribute>();
        return attribute?.Type ?? default;
    }

    public static string ToDisplayName(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<EnumDisplayAttribute>();
        return attribute?.Name ?? enumValue.ToString().GenerateDisplayName();
    }

    public static string GetEnumDisplayName(this Type enumType, object value)
    {
        if (enumType.IsEnum && value != null)
        {
            var enumValue = Convert.ToInt32(value);
            return Enum.GetValues(enumType)
                .Cast<object>()
                .Where(e => enumValue == ((Enum)e).GetEnumValue())
                .Select(e => ((Enum)e).ToDisplayName())
                .FirstOrDefault();
        }

        return null;
    }

    public static IEnumerable<string> ToNameList(this Enum enumValue)
    {
        return enumValue.ToDisplayName().Split(",").Select(w => w.Trim());
    }
    
    public static TEnum ToEnum<TEnum>(this int value) where TEnum : struct, IConvertible
    {
        if (Enum.IsDefined(typeof(TEnum), value))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }
        throw new EnumParseException($"Integer {value} has no value for enum {typeof(TEnum).FullName}.");
    }

    public static TEnum? ToEnum<TEnum>(this int? value) where TEnum : struct, IConvertible
    {
        return value?.ToEnum<TEnum>();
    }

    public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct, IConvertible
    {
        if (Enum.IsDefined(typeof(TEnum), value))
        {
            return Enum.TryParse<TEnum>(value, true, out var result) ? result : default;
        }
        throw new EnumParseException($"String {value} has no value for enum {typeof(TEnum).FullName}.");
    }

    public static int ToEnumValue<TEnum>(this string name) where TEnum : struct, IConvertible
    {
        if (string.IsNullOrEmpty(name) || !name.IsEnumName<TEnum>())
        {
            return default;
        }

        return typeof(TEnum).ToEnumItemList()
            .Where(x => x.DisplayText == name || x.Name == name)
            .Select(x => x.Value)
            .FirstOrDefault();
    }

    public static int ToEnumValueByShortName<TEnum>(this string shortName) where TEnum : struct, IConvertible
    {
        if (string.IsNullOrEmpty(shortName))
        {
            return default;
        }

        return typeof(TEnum).ToLookupEnumItemList()
            .Where(x => x.ShortName == shortName)
            .Select(x => x.Value)
            .FirstOrDefault();
    }
    
    public static int? ToNullableEnumValue<TEnum>(this string name) where TEnum : struct, IConvertible
    {
        var result = ToEnumValue<TEnum>(name);
        return result == default ? null : result;
    }

    public static int? ToNullableEnumValueByShortName<TEnum>(this string name) where TEnum : struct, IConvertible
    {
        var result = ToEnumValueByShortName<TEnum>(name);
        return result == default ? null : result;
    }

    public static string ToEnumName<TEnum>(this int value) where TEnum : struct, IConvertible
    {
        return typeof(TEnum).ToEnumItemList()
            .Where(x => x.Value == value)
            .Select(x => x.Name)
            .FirstOrDefault();
    }

    public static bool IsEnumValue<TEnum>(this string name) where TEnum : struct, IConvertible
    {
        var enumList = typeof(TEnum).ToLookupEnumItemList();
        return enumList.Any(x => x.Name == name || x.DisplayText == name);
    }

    public static bool IsEnumValue<TEnum>(this int value) where TEnum : struct, IConvertible
    {
        var enumList = typeof(TEnum).ToLookupEnumItemList();
        return enumList.Any(x => x.Value == value);
    }

    public static bool IsEnumName<TEnum>(this string name) where TEnum : struct, IConvertible
    {
        var enumList = typeof(TEnum).ToLookupEnumItemList();
        return enumList.Any(x => x.Name == name || x.DisplayText == name);
    }

    public static string GetConfigValue(this Enum enumValue)
        => enumValue.GetAttributeOfType<EnumConfigAttribute>()?.Value;

    public static int GetMinValue(this Type enumType)
    {
        if (enumType.IsEnum)
        {
            return Math.Max(1, Convert.ToInt32(Enum.GetValues(enumType).Cast<IFormattable>().Min()));
        }

        throw new EnumParseException($"{enumType} is not an enum type.");
    }

    public static int GetMaxValue(this Type enumType)
    {
        if (enumType.IsEnum)
        {
            if (enumType.GetCustomAttributes<FlagsAttribute>().Any())
            {
                return Convert.ToInt32(Enum.GetValues(enumType).Cast<int>().Sum());
            }

            return Convert.ToInt32(Enum.GetValues(enumType).Cast<IFormattable>().Max());
        }

        throw new EnumParseException($"{enumType} is not an enum type.");
    }

    public static IEnumerable<int> ToEnumValueList(this Type enumType)
    {
        return Enum.GetValues(enumType).Cast<int>();
    }

    public static IList<EnumItemModel> ToLookupEnumItemList(this Type enumType) =>
        ToEnumItemList(enumType)?.Where(x => x.Value > 0).ToList();

    public static IEnumerable<EnumItemModel> ToEnumItemList(this Type enumType)
    {
        if (enumType.IsEnum)
        {
            return Enum.GetValues(enumType)
                .Cast<object>()
                .Select(e => new EnumItemModel
                {
                    Value = ((Enum)e).GetEnumValue(),
                    RawValue = (Enum)e,
                    Name = e.ToString(),
                    DisplayText = ((Enum)e).ToDisplayName(),
                    ShortName = ((Enum)e).GetShortName(),
                    Description = ((Enum)e).GetDescription(),
                    IsDefault = ((Enum)e).GetDefaultValue(),
                    IsActive = ((Enum)e).GetActiveValue(),
                    GroupId = ((Enum)e).GetGroupValue(),
                    Order = ((Enum)e).GetOrderValue(),
                    Type = ((Enum)e).GetTypeValue(),
                });
        }

        return new List<EnumItemModel>();
    }

    public static IEnumerable<EnumItemModel> ToFlagedEnumItemList(this Type enumType)
    {
        if (enumType.IsEnum)
        {
            return Enumerable.Range(1, enumType.GetMaxValue())
                .Select(e => Enum.ToObject(enumType, e))
                .Select(e => new EnumItemModel
                {
                    Value = ((Enum)e).GetEnumValue(),
                    Name = e.ToString(),
                    DisplayText = ((Enum)e).ToString()
                }).ToList();
        }

        return new List<EnumItemModel>();
    }

    public static IEnumerable<ConfigItemModel> ToConfigItemList(this Type enumType)
    {
        if (enumType.IsEnum)
        {
            return Enum.GetValues(enumType)
                .Cast<object>()
                .Select(e => new ConfigItemModel
                {
                    Id = ((Enum)e).GetEnumValue(),
                    Name = e.ToString(),
                    Value = ((Enum)e).GetConfigValue()
                });
        }

        return new List<ConfigItemModel>();
    }

    public static string ToJoinedString<TEnum>(this IEnumerable<TEnum> enums) where TEnum : Enum
    {
        if (enums.Any())
        {
            return string.Join(", ", enums.Select(x => x.ToDisplayName()));
        }

        return string.Empty;
    }
}
