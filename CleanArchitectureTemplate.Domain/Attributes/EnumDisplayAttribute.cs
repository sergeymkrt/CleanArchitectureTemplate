namespace CleanArchitectureTemplate.Domain.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EnumDisplayAttribute : Attribute
{
    public EnumDisplayAttribute()
    {
    }

    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public int GroupId { get; set; }
    public int Order { get; set; }
    public int Type { get; set; }
    public int MappingValue { get; set; }
}
