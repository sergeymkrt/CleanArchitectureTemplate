using System.Runtime.Serialization;
using FluentValidation.Results;

namespace CleanArchitectureTemplate.Application.Exceptions;

[Serializable]
public class ValidationException : Exception, ISerializable
{
    protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {

    }
    public ValidationException()
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

    }

    public IDictionary<string, string[]> Errors { get; }
    public string FirstError => Errors.Select(x => x.Value.FirstOrDefault()).FirstOrDefault();
}
