using System.ComponentModel.DataAnnotations;
using CleanArchitectureTemplate.Domain.Attributes;
using CleanArchitectureTemplate.Domain.SeedWork;
using CleanArchitectureTemplate.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Domain.Aggregates.Users;

public class User : IdentityUser<long>, IAggregateRoot
{
    [Required]
    [MaxLength(128)]
    [SearchColumn]
    public string FirstName { get; protected set; }

    [Required]
    [MaxLength(128)]
    [SearchColumn]
    public string LastName { get; protected set; }

    public string FullName => $"{LastName} {FirstName}";

    public virtual ICollection<AuditLog> AuditLogs { get; protected set; }

    protected User() { }

    public User(
        long id,
        string firstName,
        string lastName,
        string email) : base(email)
    {
        Id = id;
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

    public User(string email)
    {
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
