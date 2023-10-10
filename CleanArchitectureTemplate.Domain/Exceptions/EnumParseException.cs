using System.Runtime.Serialization;

namespace CleanArchitectureTemplate.Domain.Exceptions;

[Serializable]
public class EnumParseException : Exception, ISerializable
{
    protected EnumParseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }

    public EnumParseException(string message) : base(message)
    {

    }
}
