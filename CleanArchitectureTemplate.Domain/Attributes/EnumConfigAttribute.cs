namespace CleanArchitectureTemplate.Domain.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EnumConfigAttribute : Attribute
{
    public EnumConfigAttribute()
    {
    }

    public string Value { get; set; }
}

