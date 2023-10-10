namespace CleanArchitectureTemplate.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SearchColumnAttribute : Attribute
{
    public SearchColumnAttribute()
    {
    }
}
