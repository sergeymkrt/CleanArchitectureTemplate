using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ResponseCodeAttribute : Attribute
{
    public ResponseCodeAttribute(ResponseType responseType, bool isFormattable = false)
    {
        ResponseType = responseType;
        IsFormattable = isFormattable;
    }

    public ResponseType ResponseType { get; set; }
    public bool IsFormattable { get; set; }
}
