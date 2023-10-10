namespace CleanArchitectureTemplate.Domain.Services.External;

public interface IIdentityService
{
    long? Id { get; }
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string FullName { get; }
    bool IsAuthenticated { get; }

    /// <summary>
    /// The system user Id for system specific tasks.
    /// </summary>
    long SystemUserId { get; }
}
