namespace CleanArchitectureTemplate.Domain.Models;

public class EnumItemModel
{
    public int Value { get; set; }
    public object RawValue { get; set; }
    public string Name { get; set; }
    public string DisplayText { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public int GroupId { get; set; }
    public int Order { get; set; }
    public int Type { get; set; }
}

