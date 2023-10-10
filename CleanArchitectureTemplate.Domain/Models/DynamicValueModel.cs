using CleanArchitectureTemplate.Domain.SeedWork;

namespace CleanArchitectureTemplate.Domain.Models;

public class DynamicValueModel : ValueObject
{
    public List<object> Values { get; set; } = new List<object>();

    public DynamicValueModel()
    {
    }

    public DynamicValueModel(object dynamicTypeObject)
    {
        Values.AddRange(dynamicTypeObject
            .GetType()
            .GetProperties()
            .Select(prop => prop.GetValue(dynamicTypeObject, null)));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var value in Values)
        {
            yield return value;
        }
    }
}
