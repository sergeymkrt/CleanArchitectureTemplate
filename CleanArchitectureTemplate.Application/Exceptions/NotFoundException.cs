﻿using System.Runtime.Serialization;

namespace CleanArchitectureTemplate.Application.Exceptions;

[Serializable]
public class NotFoundException : Exception, ISerializable
{
    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }
    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
