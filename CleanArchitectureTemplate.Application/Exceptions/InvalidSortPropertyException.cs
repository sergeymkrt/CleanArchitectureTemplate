using System.Runtime.Serialization;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Application.Exceptions;

[Serializable]
public class InvalidSortPropertyException : Exception, ISerializable
{
    protected InvalidSortPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }
    public InvalidSortPropertyException(string propertyName, SortExceptionCode code)
        : base(GetMessage(propertyName, code))
    {
        PropertyName = propertyName;
        Code = code;
    }

    public string PropertyName { get; set; }
    public SortExceptionCode Code { get; set; }

    public static string GetMessage(string propertyName, SortExceptionCode code)
    {
        return code switch
        {
            SortExceptionCode.InvalidSortingProperty => $"Invalid sorting property {propertyName}",
            SortExceptionCode.NonSortingProperty => $"{propertyName} is not a sorting property: ",
            _ => throw new Exception("Not implemented case"),
        };
    }
}
