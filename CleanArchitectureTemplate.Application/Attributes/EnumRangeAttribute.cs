using System.Collections;
using System.ComponentModel.DataAnnotations;
using CleanArchitectureTemplate.Domain.Extensions;

namespace CleanArchitectureTemplate.Application.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class EnumRangeAttribute : RangeAttribute
{
    public Type Type { get; private set; }

    public EnumRangeAttribute(Type type) : base(type.GetMinValue(), type.GetMaxValue())
    {
        Type = type;
    }

}

[AttributeUsage(AttributeTargets.Property)]
public class EnumListRangeAttribute : ValidationAttribute
{
    public Type Type { get; private set; }

    public EnumListRangeAttribute(Type type)
    {
        Type = type;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (Type.IsEnum && value is IList list)
        {
            int minValue = Type.GetMinValue();
            int maxValue = Type.GetMaxValue();

            return list.Cast<int>().All(li => minValue <= li && li <= maxValue)
                ? null
                : new ValidationResult($"The values of the field {validationContext.MemberName} must be between {minValue} and {maxValue}.");
        }

        return new ValidationResult($"Type provided must be an Enum.");
    }
}
