namespace CleanArchitectureTemplate.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SkipAuditLogAttribute : Attribute
{
    public SkipAuditLogAttribute()
    {
    }
}
