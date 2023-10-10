using System.ComponentModel.DataAnnotations;
using CleanArchitectureTemplate.Domain.Attributes;
using CleanArchitectureTemplate.Domain.SeedWork;
using CleanArchitectureTemplate.Domain.Shared;

namespace CleanArchitectureTemplate.Domain.Aggregates.Users;

public class User : BaseEntity<long>, IAggregateRoot
{
    [Required]
    [MaxLength(128)]
    [SearchColumn]
    public string FirstName { get; protected set; }

    [Required]
    [MaxLength(128)]
    [SearchColumn]
    public string LastName { get; protected set; }

    [Required]
    [MaxLength(128)]
    public string Email { get; protected set; }

    public string FullName => $"{LastName} {FirstName}";

    public virtual ICollection<AuditLog> AuditLogs { get; protected set; }

    protected User() { }

    public User(
        long id,
        string firstName,
        string lastName,
        string email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public User(
        string firstName,
        string lastName,
        string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public void SetName(
        string firstName,
        string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
