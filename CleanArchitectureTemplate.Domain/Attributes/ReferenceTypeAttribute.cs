using CleanArchitectureTemplate.Domain.Extensions;
using CleanArchitectureTemplate.Domain.SeedWork;

namespace CleanArchitectureTemplate.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ReferenceTypeAttribute : Attribute
{
    public Type Type { get; private set; }

    public ReferenceTypeAttribute(Type type)
    {
        Type = type;
    }

    #region Methods
    public bool IsEnum() => Type.IsEnum;
    public bool IsEntity() => Type.IsSubclassOfRawGeneric(typeof(BaseEntity<>));
    #endregion Methods
}

